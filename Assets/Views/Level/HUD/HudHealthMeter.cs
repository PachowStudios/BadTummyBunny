using System.Linq;
using System.Linq.Extensions;
using MarkLight;
using MarkLight.Views.UI;
using Zenject;

namespace PachowStudios.BadTummyBunny.UI
{
  [HideInPresenter]
  public class HudHealthMeter : UIView,
    IHandles<PlayerHealthChangedMessage>,
    IHandles<PlayerHealthContainersChangedMessage>
  {
    [DataBound] public ObservableList<Field<int>> Hearts { get; set; } =
      Enumerable.Repeat(new Field<int>(4), 3).ToObservableList();

    [Inject] private IEventAggregator EventAggregator { get; set; }

    private int HealthPerContainer { get; set; }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public void Handle(PlayerHealthChangedMessage message)
    {
      var fullContainers = message.Health / HealthPerContainer;

      for (var i = 0; i < Hearts.Count; i++)
      {
        var health = 0;

        if (i < fullContainers)
          health = HealthPerContainer;
        else if (i == fullContainers)
          health = message.Health % HealthPerContainer;

        Hearts[i].Value = health;
      }

      Hearts.ItemsModified();
    }

    public void Handle(PlayerHealthContainersChangedMessage message)
    {
      HealthPerContainer = message.HealthPerContainer;

      while (Hearts.HasLessThan(message.HealthContainers))
        Hearts.Add(0);

      while (Hearts.HasMoreThan(message.HealthContainers))
        Hearts.Pop();
    }
  }
}