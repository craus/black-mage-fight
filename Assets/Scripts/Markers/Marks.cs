using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Marks : Singletone<Marks>
{
	public Mark monster;
	public static Mark Monster { get { return instance.monster; } }

	public Mark bomb;
	public static Mark Bomb { get { return instance.bomb; } }

	public Mark blackMageRelocator;
	public static Mark BlackMageRelocator { get { return instance.blackMageRelocator; } }
}
