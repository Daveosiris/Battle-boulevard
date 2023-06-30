using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	public abstract class ComponentBase : MonoBehaviour
	{
		[NonSerialized]
		private bool isReady;

		[NonSerialized]
		private bool isDestroyed;

		public bool IsInitialized => isReady;

		public bool IsDestroyed => isDestroyed;

		protected abstract void OnInitComponent();

		protected abstract void OnDestroyComponent();

		protected abstract void OnEnableComponent();

		protected abstract void OnDisableComponent();

		public bool CanBeUsed()
		{
			if (isDestroyed)
			{
				return false;
			}
			if (!isReady)
			{
				isReady = true;
				OnInitComponent();
			}
			return true;
		}

		public void Init()
		{
			if (!isReady)
			{
				isReady = true;
				OnInitComponent();
			}
		}

		public void ForceInit()
		{
			isReady = true;
			OnInitComponent();
		}

		private void OnEnable()
		{
			Init();
			OnEnableComponent();
		}

		private void OnDisable()
		{
			OnDisableComponent();
		}

		private void OnDestroy()
		{
			isDestroyed = true;
			isReady = false;
			OnDestroyComponent();
		}
	}
}
