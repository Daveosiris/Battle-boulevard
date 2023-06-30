using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	public struct BindingDescription
	{
		[Flags]
		public enum BindingType
		{
			Digital = 0x1,
			Axis = 0x2,
			EmuTouch = 0x20,
			MousePos = 0x40,
			All = 0x7F
		}

		public BindingType type;

		public string name;

		public string nameFormatted;

		public string menuPath;

		public InputBindingBase binding;

		public InputRig.InputSource axisSource;

		public UnityEngine.Object undoObject;
	}
}
