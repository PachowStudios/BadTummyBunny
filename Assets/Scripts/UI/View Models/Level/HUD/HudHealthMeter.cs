using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using MarkUX;
using MarkUX.Views;
using Zenject;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class HudHealthMeter : View,
    IHandles<PlayerHealthChangedMessage>,
    IHandles<PlayerHealthContainersChangedMessage>
  {
    [DataBound, ChangeHandler(nameof(UpdateLayout))]
    public List<Field<int>> Hearts = new Field<int>[] { 4, 4, 4 }.ToList();

    [DataBound] public List HeartList = null;

    [Inject] private IEventAggregator EventAggregator { get; set; }

    private int HealthPerContainer { get; set; }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public void Handle(PlayerHealthChangedMessage message)
    {
      var fullContainers = message.Health / HealthPerContainer;

      for (var i = 0; i < this.Hearts.Count; i++)
        this.Hearts[i].Value =
          i < fullContainers ? HealthPerContainer
            : i == fullContainers ? message.Health % HealthPerContainer
              : 0;

      this.HeartList.UpdateItemBindings();
    }

    public void Handle(PlayerHealthContainersChangedMessage message)
    {
      HealthPerContainer = message.HealthPerContainer;

      while (this.Hearts.HasLessThan(message.HealthContainers))
        this.Hearts.Add(0);

      while (this.Hearts.HasMoreThan(message.HealthContainers))
        this.Hearts.Pop();

      SetChanged(() => this.Hearts);
    }
  }
}