﻿public interface ICharacter
{
	IMovable Movement { get; }
	IHasHealth Health { get; }
}