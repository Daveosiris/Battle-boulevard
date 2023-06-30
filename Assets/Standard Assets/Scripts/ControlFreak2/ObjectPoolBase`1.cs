using System;
using System.Collections.Generic;

namespace ControlFreak2
{
	public abstract class ObjectPoolBase<T>
	{
		protected List<T> usedList;

		protected List<T> unusedList;

		public ObjectPoolBase()
		{
			usedList = new List<T>();
			unusedList = new List<T>();
		}

		protected abstract T CreateInternalObject();

		protected virtual void OnDestroyObject(T obj)
		{
		}

		protected virtual void OnUseObject(T obj)
		{
		}

		protected virtual void OnUnuseObeject(T obj)
		{
		}

		public int GetAllocatedCount()
		{
			return usedList.Count + unusedList.Count;
		}

		public int GetUsedCount()
		{
			return usedList.Count;
		}

		public int GetUnusedCount()
		{
			return unusedList.Count;
		}

		public List<T> GetList()
		{
			return usedList;
		}

		public T GetNewObject(int insertPos = -1)
		{
			if (unusedList.Count == 0)
			{
				return default(T);
			}
			T val = unusedList[unusedList.Count - 1];
			unusedList.RemoveAt(unusedList.Count - 1);
			OnUseObject(val);
			if (insertPos < 0)
			{
				usedList.Add(val);
			}
			else
			{
				usedList.Insert(insertPos, val);
			}
			return val;
		}

		public void UnuseObject(T obj)
		{
			int num = usedList.IndexOf(obj);
			if (num >= 0)
			{
				usedList.RemoveAt(num);
				OnUnuseObeject(obj);
				unusedList.Add(obj);
			}
		}

		public void Clear()
		{
			for (int i = 0; i < usedList.Count; i++)
			{
				OnUnuseObeject(usedList[i]);
			}
			unusedList.AddRange(usedList);
			usedList.Clear();
		}

		public void Trim(int maxCount, bool trimAtEnd = true)
		{
			if (maxCount < 0)
			{
				maxCount = 0;
			}
			int num = GetUsedCount() - maxCount;
			if (num > 0)
			{
				int num2 = trimAtEnd ? (usedList.Count - num) : 0;
				int num3 = num2 + num;
				for (int i = num2; i < num3; i++)
				{
					T val = usedList[i];
					OnUnuseObeject(val);
					unusedList.Add(val);
				}
				usedList.RemoveRange(num2, num);
			}
		}

		public void EnsureCapacity(int count)
		{
			if (GetAllocatedCount() < count)
			{
				Allocate(count);
			}
		}

		public void Allocate(int count)
		{
			if (count < 0)
			{
				count = 0;
			}
			if (count == GetAllocatedCount())
			{
				return;
			}
			if (count > GetAllocatedCount())
			{
				if (usedList.Capacity < count)
				{
					usedList.Capacity = count;
				}
				if (unusedList.Capacity < count)
				{
					unusedList.Capacity = count;
				}
				int num = count - GetAllocatedCount();
				int num2 = 0;
				while (true)
				{
					if (num2 < num)
					{
						T val = CreateInternalObject();
						if (val == null)
						{
							break;
						}
						unusedList.Add(val);
						num2++;
						continue;
					}
					return;
				}
				throw new Exception("Could not create a new object pool element [" + typeof(T).ToString() + "]!");
			}
			int num3 = -(count - unusedList.Count);
			if (num3 > 0)
			{
				int num4 = usedList.Count - num3;
				for (int i = num4; i < usedList.Count; i++)
				{
					T val2 = usedList[i];
					OnUnuseObeject(val2);
					unusedList.Add(val2);
				}
				usedList.RemoveRange(num4, num3);
			}
			int num5 = unusedList.Count - count;
			int num6 = unusedList.Count - num5;
			for (int j = num6; j < unusedList.Count; j++)
			{
				OnDestroyObject(unusedList[j]);
			}
			unusedList.RemoveRange(num6, num5);
		}

		public void DestroyAll()
		{
			Allocate(0);
		}
	}
}
