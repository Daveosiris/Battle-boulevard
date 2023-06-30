using System;
using UnityEngine;

namespace ControlFreak2
{
	public abstract class GameState : MonoBehaviour
	{
		protected GameState parentState;

		protected GameState subState;

		protected bool isRunning;

		public bool IsRunning()
		{
			return isRunning;
		}

		protected virtual void OnStartState(GameState parentState)
		{
			this.parentState = parentState;
			isRunning = true;
		}

		protected virtual void OnExitState()
		{
			isRunning = false;
			if (subState != null)
			{
				subState.OnExitState();
			}
		}

		protected virtual void OnPreSubStateStart(GameState prevState, GameState nextState)
		{
		}

		protected virtual void OnPostSubStateStart(GameState prevState, GameState nextState)
		{
		}

		protected virtual void OnUpdateState()
		{
			if (subState != null)
			{
				subState.OnUpdateState();
			}
		}

		protected virtual void OnFixedUpdateState()
		{
			if (subState != null)
			{
				subState.OnFixedUpdateState();
			}
		}

		public void StartSubState(GameState state)
		{
			if (FindStateInHierarchy(state))
			{
				throw new Exception("Gamestate (" + base.name + ") tries to start sub state (" + state.name + ") that's already running!");
			}
			GameState prevState = subState;
			OnPreSubStateStart(prevState, state);
			if (subState != null)
			{
				subState.OnExitState();
			}
			if ((subState = state) != null)
			{
				subState.OnStartState(this);
			}
			OnPostSubStateStart(prevState, state);
		}

		protected bool FindStateInHierarchy(GameState state)
		{
			if (state == null)
			{
				return false;
			}
			GameState gameState = this;
			while (gameState != null)
			{
				if (gameState == state)
				{
					return true;
				}
				gameState = gameState.parentState;
			}
			return false;
		}

		public void EndState()
		{
			if (parentState != null)
			{
				parentState.EndSubState();
			}
		}

		public void EndSubState()
		{
			StartSubState(null);
		}

		public GameState GetSubState()
		{
			return subState;
		}

		public bool IsSubStateRunning()
		{
			return subState != null;
		}
	}
}
