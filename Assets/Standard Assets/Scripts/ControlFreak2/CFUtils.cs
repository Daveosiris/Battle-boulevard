using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ControlFreak2
{
	public static class CFUtils
	{
		public const float SqrtOf2 = 1.41421354f;

		public const float SqrtOf3 = 1.73205078f;

		public const float OneOverSqrtOf2 = 0.707106769f;

		public const float OneOverSqrtOf3 = 0.577350259f;

		public const float TanLenFor45Tri = 0.765366852f;

		public const float TanLenFor90Tri = 1.41421354f;

		public const float MAX_DELTA_TIME = 0.5f;

		private const float MAX_LEFT_FACTOR = 0.75f;

		public static float realDeltaTime => (Time.captureFramerate > 0) ? (1f / (float)Time.captureFramerate) : Time.unscaledDeltaTime;

		public static float realDeltaTimeClamped => Mathf.Min(realDeltaTime, 0.5f);

		public static bool editorStopped => false;

		public static bool forcedMobileModeEnabled => true;

		public static string LogPrefixFull()
		{
			return "[" + Time.frameCount + "] ";
		}

		public static string LogPrefix()
		{
			return "[" + Time.frameCount + "] ";
		}

		public static float ApplyDeltaInput(float accum, float delta)
		{
			if (accum >= 0f && delta >= 0f)
			{
				return Mathf.Max(accum, delta);
			}
			if (accum < 0f && delta < 0f)
			{
				return Mathf.Min(accum, delta);
			}
			return accum + delta;
		}

		public static int ApplyDeltaInputInt(int accum, int delta)
		{
			if (accum >= 0 && delta >= 0)
			{
				return Mathf.Max(accum, delta);
			}
			if (accum < 0 && delta < 0)
			{
				return Mathf.Min(accum, delta);
			}
			return accum + delta;
		}

		public static float ApplyPositveDeltaInput(float positiveAccum, float delta)
		{
			return (!(delta > 0f) || !(delta > positiveAccum)) ? positiveAccum : delta;
		}

		public static float ApplyNegativeDeltaInput(float negativeAccum, float delta)
		{
			return (!(delta < 0f) || !(delta < negativeAccum)) ? negativeAccum : delta;
		}

		public static void ApplySignedDeltaInput(float v, ref float plusAccum, ref float minusAccum)
		{
			if (v >= 0f)
			{
				if (v > plusAccum)
				{
					plusAccum = v;
				}
			}
			else if (v < minusAccum)
			{
				minusAccum = v;
			}
		}

		public static void ApplySignedDeltaInputInt(int v, ref int plusAccum, ref int minusAccum)
		{
			if (v >= 0)
			{
				if (v > plusAccum)
				{
					plusAccum = v;
				}
			}
			else if (v < minusAccum)
			{
				minusAccum = v;
			}
		}

		public static int GetScrollValue(float drag, int prevScroll, float thresh, float magnet)
		{
			bool flag = drag < 0f;
			if (flag)
			{
				drag = 0f - drag;
				prevScroll = -prevScroll;
			}
			float f = drag / thresh;
			int num = Mathf.FloorToInt(f);
			return (!flag) ? num : (-num);
		}

		public static float MoveTowards(float a, float b, float secondsPerUnit, float deltaTime, float epsilon)
		{
			if (Mathf.Abs(b - a) <= epsilon)
			{
				return b;
			}
			return (!(secondsPerUnit < 0.001f) && !(secondsPerUnit <= deltaTime)) ? Mathf.MoveTowards(a, b, deltaTime / secondsPerUnit) : b;
		}

		private static float GetLerpFactor(float smoothingTime, float deltaTime, float maxLerpFactor)
		{
			return Mathf.Min(maxLerpFactor, (!(smoothingTime <= deltaTime)) ? (deltaTime / smoothingTime) : 1f);
		}

		public static float SmoothTowards(float a, float b, float smoothingTime, float deltaTime, float epsilon, float maxLeftFactor = 0.75f)
		{
			if (Mathf.Abs(b - a) <= epsilon)
			{
				return b;
			}
			return (!(smoothingTime < 0.001f)) ? Mathf.Lerp(a, b, GetLerpFactor(smoothingTime, deltaTime, maxLeftFactor)) : b;
		}

		public static float SmoothTowardsAngle(float a, float b, float smoothingTime, float deltaTime, float epsilon, float maxLeftFactor = 0.75f)
		{
			if (Mathf.Abs(Mathf.DeltaAngle(b, a)) <= epsilon)
			{
				return b;
			}
			return (!(smoothingTime < 0.001f)) ? Mathf.MoveTowardsAngle(a, b, Mathf.Abs(b - a) * GetLerpFactor(smoothingTime, deltaTime, maxLeftFactor)) : b;
		}

		public static Vector2 SmoothTowardsVec2(Vector2 a, Vector2 b, float smoothingTime, float deltaTime, float sqrEpsilon, float maxLeftFactor = 0.75f)
		{
			if (sqrEpsilon != 0f && (b - a).sqrMagnitude <= sqrEpsilon)
			{
				return b;
			}
			return (!(smoothingTime < 0.001f)) ? Vector2.Lerp(a, b, GetLerpFactor(smoothingTime, deltaTime, maxLeftFactor)) : b;
		}

		public static Vector3 SmoothTowardsVec3(Vector3 a, Vector3 b, float smoothingTime, float deltaTime, float sqrEpsilon, float maxLeftFactor = 0.75f)
		{
			if (sqrEpsilon != 0f && (b - a).sqrMagnitude <= sqrEpsilon)
			{
				return b;
			}
			return (!(smoothingTime < 0.001f)) ? Vector3.Lerp(a, b, GetLerpFactor(smoothingTime, deltaTime, maxLeftFactor)) : b;
		}

		public static Color SmoothTowardsColor(Color a, Color b, float smoothingTime, float deltaTime, float maxLeftFactor = 0.75f)
		{
			return (!(smoothingTime < 0.001f)) ? Color.Lerp(a, b, GetLerpFactor(smoothingTime, deltaTime, maxLeftFactor)) : b;
		}

		public static Quaternion SmoothTowardsQuat(Quaternion a, Quaternion b, float smoothingTime, float deltaTime, float maxLeftFactor = 0.75f)
		{
			return (!(smoothingTime < 0.001f)) ? Quaternion.Slerp(a, b, GetLerpFactor(smoothingTime, deltaTime, maxLeftFactor)) : b;
		}

		public static float SmoothDamp(float valFrom, float valTo, ref float vel, float smoothingTime, float deltaTime, float epsilon)
		{
			if (smoothingTime < 0.001f || Mathf.Abs(valTo - valFrom) <= epsilon)
			{
				vel = 0f;
				return valTo;
			}
			return Mathf.SmoothDamp(valFrom, valTo, ref vel, smoothingTime, 1E+07f, deltaTime);
		}

		public static Color ScaleColorAlpha(Color color, float alphaScale)
		{
			color.a *= alphaScale;
			return color;
		}

		public static Component GetComponentHereOrInParent(Component comp, Type compType)
		{
			if (comp == null)
			{
				return null;
			}
			Component component = null;
			return (!((component = comp.GetComponent(compType)) != null)) ? comp.GetComponentInParent(compType) : component;
		}

		public static bool IsStretchyRectTransform(Transform tr)
		{
			RectTransform rectTransform = tr as RectTransform;
			return rectTransform != null && rectTransform.anchorMax != rectTransform.anchorMin;
		}

		public static Rect TransformRect(Rect r, Matrix4x4 tr, bool round)
		{
			Bounds bounds = TransformRectAsBounds(r, tr, round);
			Vector3 min = bounds.min;
			Vector3 size = bounds.size;
			return new Rect(min.x, min.y, size.x, size.y);
		}

		public static Bounds TransformRectAsBounds(Rect r, Matrix4x4 tr, bool round)
		{
			Vector3 a = tr.MultiplyPoint3x4(r.center);
			Vector3 a2 = tr.MultiplyVector(new Vector3(1f, 0f, 0f));
			Vector3 a3 = tr.MultiplyVector(new Vector3(0f, 1f, 0f));
			float d = r.width * 0.5f;
			float d2 = r.height * 0.5f;
			a2 *= d;
			a3 *= d2;
			Vector3 lhs;
			Vector3 lhs2;
			if (round)
			{
				Vector3 vector = a3;
				lhs = Vector3.Min(vector, -vector);
				lhs2 = Vector3.Max(vector, -vector);
				vector = a2 * 0.77f + a3 * 0.77f;
				lhs = Vector3.Min(lhs, Vector3.Min(vector, -vector));
				lhs2 = Vector3.Max(lhs2, Vector3.Max(vector, -vector));
				vector = a2;
				lhs = Vector3.Min(lhs, Vector3.Min(vector, -vector));
				lhs2 = Vector3.Max(lhs2, Vector3.Max(vector, -vector));
				vector = a2 * -0.77f + a3 * 0.77f;
				lhs = Vector3.Min(lhs, Vector3.Min(vector, -vector));
				lhs2 = Vector3.Max(lhs2, Vector3.Max(vector, -vector));
			}
			else
			{
				Vector3 vector = a2 + a3;
				lhs = Vector3.Min(vector, -vector);
				lhs2 = Vector3.Max(vector, -vector);
				vector = a2 - a3;
				lhs = Vector3.Min(lhs, Vector3.Min(vector, -vector));
				lhs2 = Vector3.Max(lhs2, Vector3.Max(vector, -vector));
			}
			return new Bounds(a + (lhs + lhs2) * 0.5f, lhs2 - lhs);
		}

		public static Matrix4x4 ChangeMatrixTranl(Matrix4x4 m, Vector3 newTransl)
		{
			m.SetColumn(3, new Vector4(newTransl.x, newTransl.y, newTransl.z, m.m33));
			return m;
		}

		public static Bounds GetWorldAABB(Matrix4x4 tf, Vector3 center, Vector3 size)
		{
			Vector3 vector = center - size * 0.5f;
			Vector3 vector2 = center + size * 0.5f;
			Vector3 vector3 = tf.MultiplyPoint3x4(new Vector3(vector.x, vector.y, vector.z));
			Vector3 lhs;
			Vector3 lhs2 = lhs = vector3;
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector.x, vector.y, vector2.z));
			lhs2 = Vector3.Min(lhs2, vector3);
			lhs = Vector3.Max(lhs, vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector.x, vector2.y, vector.z));
			lhs2 = Vector3.Min(lhs2, vector3);
			lhs = Vector3.Max(lhs, vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector.x, vector2.y, vector2.z));
			lhs2 = Vector3.Min(lhs2, vector3);
			lhs = Vector3.Max(lhs, vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector2.x, vector.y, vector.z));
			lhs2 = Vector3.Min(lhs2, vector3);
			lhs = Vector3.Max(lhs, vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector2.x, vector2.y, vector.z));
			lhs2 = Vector3.Min(lhs2, vector3);
			lhs = Vector3.Max(lhs, vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector2.x, vector.y, vector2.z));
			lhs2 = Vector3.Min(lhs2, vector3);
			lhs = Vector3.Max(lhs, vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector2.x, vector2.y, vector2.z));
			lhs2 = Vector3.Min(lhs2, vector3);
			lhs = Vector3.Max(lhs, vector3);
			return new Bounds((lhs2 + lhs) * 0.5f, lhs - lhs2);
		}

		public static Bounds GetWorldAABB(Matrix4x4 tf, Bounds localBounds)
		{
			return GetWorldAABB(tf, localBounds.center, localBounds.size);
		}

		public static Rect GetWorldRect(Matrix4x4 tf, Bounds localBounds)
		{
			Bounds worldAABB = GetWorldAABB(tf, localBounds);
			Vector3 min = worldAABB.min;
			Vector3 max = worldAABB.max;
			return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
		}

		public static Vector2 ClampRectInside(Rect rect, bool rectIsRound, Rect limiterRect, bool limiterIsRound)
		{
			if (limiterIsRound)
			{
				if (rectIsRound)
				{
					return ClampEllipseInsideEllipse(rect, limiterRect);
				}
				return ClampRectInsideEllipse(rect, limiterRect);
			}
			return ClampRectInsideRect(rect, limiterRect);
		}

		public static Vector2 ClampRectInsideEllipse(Rect rect, Rect limiterRect)
		{
			return ClampEllipseInsideEllipse(rect, limiterRect);
		}

		public static Vector2 ClampEllipseInsideEllipse(Rect rect, Rect limiterRect)
		{
			Vector2 b = rect.size * 0.5f;
			Vector2 a = limiterRect.size * 0.5f;
			Vector2 center = rect.center;
			Vector2 center2 = limiterRect.center;
			if (b.x >= a.x || b.y >= a.y)
			{
				Vector2 result = default(Vector2);
				if (b.x >= a.x)
				{
					result.x = center2.x - center.x;
				}
				else
				{
					result.x = ((!(center.x >= center2.x)) ? Mathf.Max(0f, center2.x - a.x - (center.x - b.x)) : Mathf.Min(0f, center2.x + a.x - (center.x + b.x)));
				}
				if (b.y >= a.y)
				{
					result.y = center2.y - center.y;
				}
				else
				{
					result.y = ((!(center.y >= center2.y)) ? Mathf.Max(0f, center2.y - a.y - (center.y - b.y)) : Mathf.Min(0f, center2.y + a.y - (center.y + b.y)));
				}
				return result;
			}
			Vector2 vector = a - b;
			Vector2 vector2 = center - center2;
			Vector2 a2 = vector2;
			a2.x /= vector.x;
			a2.y /= vector.y;
			if (a2.sqrMagnitude < 1f)
			{
				return Vector2.zero;
			}
			a2.Normalize();
			a2.x *= vector.x;
			a2.y *= vector.y;
			return a2 - vector2;
		}

		public static Vector2 ClampRectInsideRect(Rect rect, Rect limiterRect)
		{
			Vector2 center = rect.center;
			Vector2 center2 = limiterRect.center;
			Vector2 b = rect.size * 0.5f;
			Vector2 b2 = limiterRect.size * 0.5f;
			Vector2 vector = center - b;
			Vector2 vector2 = center + b;
			Vector2 vector3 = center2 - b2;
			Vector2 vector4 = center2 + b2;
			Vector2 zero = Vector2.zero;
			if (b.x >= b2.x)
			{
				zero.x = center2.x - center.x;
			}
			else if (vector2.x > vector4.x)
			{
				zero.x = vector4.x - vector2.x;
			}
			else if (vector.x < vector3.x)
			{
				zero.x = vector3.x - vector.x;
			}
			if (b.y >= b2.y)
			{
				zero.y = center2.y - center.y;
			}
			else if (vector2.y > vector4.y)
			{
				zero.y = vector4.y - vector2.y;
			}
			else if (vector.y < vector3.y)
			{
				zero.y = vector3.y - vector.y;
			}
			return zero;
		}

		public static Vector2 ClampInsideUnitCircle(Vector2 np)
		{
			return (!(np.sqrMagnitude < 1f)) ? np.normalized : np;
		}

		public static Vector2 ClampInsideUnitSquare(Vector2 np)
		{
			float num = 1f;
			if (np.x > 1f || np.x < -1f)
			{
				num = Mathf.Abs(1f / np.x);
			}
			if (np.y > 1f || np.y < -1f)
			{
				num = Mathf.Min(num, Mathf.Abs(1f / np.y));
			}
			return (!(num < 1f)) ? np : (np * num);
		}

		public static Vector2 ClampPerAxisInsideUnitSquare(Vector2 np)
		{
			np.x = Mathf.Clamp(np.x, -1f, 1f);
			np.y = Mathf.Clamp(np.y, -1f, 1f);
			return np;
		}

		public static Vector2 ClampInsideRect(Vector2 v, Rect r)
		{
			if (r.Contains(v))
			{
				return v;
			}
			Vector2 center = r.center;
			Vector2 vector = r.size * 0.5f;
			v -= center;
			float num = 1f;
			if (v.x > vector.x || v.x < 0f - vector.x)
			{
				num = Mathf.Abs(vector.x / v.x);
			}
			if (v.y > vector.y || v.y < 0f - vector.y)
			{
				num = Mathf.Min(num, Mathf.Abs(vector.y / v.y));
			}
			return center + v * num;
		}

		public static bool IsDirDiagonal(Dir dir)
		{
			return ((dir - 1) & Dir.U) == Dir.U;
		}

		public static float VecToAngle(Vector2 vec)
		{
			return NormalizeAnglePositive(Mathf.Atan2(vec.x, vec.y) * 57.29578f);
		}

		public static float VecToAngle(Vector2 vec, float defaultAngle, float deadZoneSq)
		{
			float sqrMagnitude = vec.sqrMagnitude;
			if (sqrMagnitude < deadZoneSq)
			{
				return defaultAngle;
			}
			if (Mathf.Abs(sqrMagnitude - 1f) > 0.0001f)
			{
				vec.Normalize();
			}
			return NormalizeAnglePositive(Mathf.Atan2(vec.x, vec.y) * 57.29578f);
		}

		public static float DirToAngle(Dir d)
		{
			if (d < Dir.U || d > Dir.UL)
			{
				return 0f;
			}
			return (float)(d - 1) * 45f;
		}

		public static Dir DirFromAngle(float ang, bool as8way)
		{
			ang += ((!as8way) ? 45f : 22.5f);
			ang = NormalizeAnglePositive(ang);
			Dir dir = Dir.N;
			if (as8way)
			{
				if (ang < 45f)
				{
					return Dir.U;
				}
				if (ang < 90f)
				{
					return Dir.UR;
				}
				if (ang < 135f)
				{
					return Dir.R;
				}
				if (ang < 180f)
				{
					return Dir.DR;
				}
				if (ang < 225f)
				{
					return Dir.D;
				}
				if (ang < 270f)
				{
					return Dir.DL;
				}
				if (ang < 315f)
				{
					return Dir.L;
				}
				return Dir.UL;
			}
			if (ang < 90f)
			{
				return Dir.U;
			}
			if (ang < 180f)
			{
				return Dir.R;
			}
			if (ang < 270f)
			{
				return Dir.D;
			}
			return Dir.L;
		}

		public static Dir DirFromAngleEx(float ang, bool as8way, Dir lastDir, float magnetPow)
		{
			if (lastDir != 0 && magnetPow > 0.001f && Mathf.Abs(Mathf.DeltaAngle(ang, DirToAngle(lastDir))) < (1f + Mathf.Clamp01(magnetPow) * 0.5f) * ((!as8way) ? 45f : 22.5f))
			{
				return lastDir;
			}
			return DirFromAngle(ang, as8way);
		}

		public static int DirDeltaAngle(Dir dirFrom, Dir dirTo)
		{
			if (dirFrom == Dir.N || dirTo == Dir.N)
			{
				return 0;
			}
			return Mathf.RoundToInt(Mathf.DeltaAngle(DirToAngle(dirFrom), DirToAngle(dirTo)));
		}

		public static Vector2 DirToNormal(Dir dir)
		{
			switch (dir)
			{
			case Dir.U:
				return new Vector2(0f, 1f);
			case Dir.R:
				return new Vector2(1f, 0f);
			case Dir.D:
				return new Vector2(0f, -1f);
			case Dir.L:
				return new Vector2(-1f, 0f);
			case Dir.UR:
				return new Vector2(0.707106769f, 0.707106769f);
			case Dir.DR:
				return new Vector2(0.707106769f, -0.707106769f);
			case Dir.DL:
				return new Vector2(-0.707106769f, -0.707106769f);
			case Dir.UL:
				return new Vector2(-0.707106769f, 0.707106769f);
			default:
				return Vector2.zero;
			}
		}

		public static Vector2 DirToTangent(Dir dir)
		{
			switch (dir)
			{
			case Dir.U:
				return new Vector2(1f, 0f);
			case Dir.R:
				return new Vector2(0f, -1f);
			case Dir.D:
				return new Vector2(-1f, 0f);
			case Dir.L:
				return new Vector2(0f, 1f);
			case Dir.UR:
				return new Vector2(0.707106769f, -0.707106769f);
			case Dir.DR:
				return new Vector2(-0.707106769f, -0.707106769f);
			case Dir.DL:
				return new Vector2(-0.707106769f, 0.707106769f);
			case Dir.UL:
				return new Vector2(0.707106769f, 0.707106769f);
			default:
				return Vector2.zero;
			}
		}

		public static Dir GetOppositeDir(Dir dir)
		{
			switch (dir)
			{
			case Dir.U:
				return Dir.D;
			case Dir.UR:
				return Dir.DL;
			case Dir.R:
				return Dir.L;
			case Dir.DR:
				return Dir.UL;
			case Dir.D:
				return Dir.U;
			case Dir.DL:
				return Dir.UR;
			case Dir.L:
				return Dir.R;
			case Dir.UL:
				return Dir.DR;
			default:
				return Dir.N;
			}
		}

		public static Vector2 DirToVector(Dir dir, bool circular)
		{
			switch (dir)
			{
			case Dir.U:
				return new Vector2(0f, 1f);
			case Dir.R:
				return new Vector2(1f, 0f);
			case Dir.D:
				return new Vector2(0f, -1f);
			case Dir.L:
				return new Vector2(-1f, 0f);
			case Dir.UR:
				return (!circular) ? new Vector2(1f, 1f) : new Vector2(0.707106769f, 0.707106769f).normalized;
			case Dir.DR:
				return (!circular) ? new Vector2(1f, -1f) : new Vector2(0.707106769f, -0.707106769f).normalized;
			case Dir.DL:
				return (!circular) ? new Vector2(-1f, -1f) : new Vector2(-0.707106769f, -0.707106769f).normalized;
			case Dir.UL:
				return (!circular) ? new Vector2(-1f, 1f) : new Vector2(-0.707106769f, 0.707106769f).normalized;
			default:
				return Vector2.zero;
			}
		}

		public static Dir VecToDir(Vector2 vec, bool as8way)
		{
			return DirFromAngle(VecToAngle(vec), as8way);
		}

		public static Dir VecToDir(Vector2 vec, Dir defaultDir, float deadZoneSq, bool as8way)
		{
			float sqrMagnitude = vec.sqrMagnitude;
			if (sqrMagnitude <= deadZoneSq)
			{
				return defaultDir;
			}
			if (Mathf.Abs(sqrMagnitude - 1f) > 1E-05f)
			{
				vec.Normalize();
			}
			return DirFromAngle(VecToAngle(vec), as8way);
		}

		public static float NormalizeAnglePositive(float a)
		{
			if (a >= 360f)
			{
				return Mathf.Repeat(a, 360f);
			}
			if (a >= 0f)
			{
				return a;
			}
			if (a <= -360f)
			{
				a = Mathf.Repeat(a, 360f);
			}
			return 360f + a;
		}

		public static float SmartDeltaAngle(float startAngle, float curAngle, float lastDelta)
		{
			float num = Mathf.DeltaAngle(startAngle + lastDelta, curAngle);
			return lastDelta + num;
		}

		public static Dir DigitalToDir(bool digiU, bool digiR, bool digiD, bool digiL)
		{
			if (digiU && digiD)
			{
				digiU = (digiD = false);
			}
			if (digiR && digiL)
			{
				digiR = (digiL = false);
			}
			if (digiU)
			{
				return digiR ? Dir.UR : ((!digiL) ? Dir.U : Dir.UL);
			}
			if (digiD)
			{
				return digiR ? Dir.DR : ((!digiL) ? Dir.D : Dir.DL);
			}
			return digiR ? Dir.R : (digiL ? Dir.L : Dir.N);
		}

		public static Vector2 CircularToSquareJoystickVec(Vector2 circularVec)
		{
			if (circularVec.sqrMagnitude < 1E-05f)
			{
				return Vector2.zero;
			}
			Vector2 vector = circularVec;
			Vector2 normalized = circularVec.normalized;
			vector *= Mathf.Abs(normalized.x) + Mathf.Abs(normalized.y);
			vector.x = Mathf.Clamp(vector.x, -1f, 1f);
			vector.y = Mathf.Clamp(vector.y, -1f, 1f);
			return vector;
		}

		public static Vector2 SquareToCircularJoystickVec(Vector2 squareVec)
		{
			if (squareVec.sqrMagnitude < 1E-05f)
			{
				return Vector2.zero;
			}
			Vector2 vector = squareVec;
			Vector2 normalized = squareVec.normalized;
			vector /= Mathf.Abs(normalized.x) + Mathf.Abs(normalized.y);
			vector.x = Mathf.Clamp(vector.x, -1f, 1f);
			vector.y = Mathf.Clamp(vector.y, -1f, 1f);
			return vector;
		}

		public static int GetLineNumber(string str, int index)
		{
			int num = 1;
			int startIndex = 0;
			int num2 = -1;
			while ((num2 = str.IndexOf('\n', startIndex)) >= 0 && num2 < index)
			{
				num++;
				startIndex = num2 + 1;
			}
			return num;
		}

		public static int GetEnumMaxValue(Type enumType)
		{
			int num = 0;
			IEnumerator enumerator = Enum.GetValues(enumType).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					int num2 = (int)enumerator.Current;
					num = ((num != 0) ? Mathf.Max(num, num2) : num2);
				}
				return num;
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public static int CycleInt(int curId, int dir, int maxId)
		{
			curId += dir;
			if (curId < 0)
			{
				curId = maxId;
			}
			else if (curId > maxId)
			{
				curId = 0;
			}
			return curId;
		}

		public static void SetEventSystemSelectedObject(GameObject o)
		{
			EventSystem current = EventSystem.current;
			if (current != null)
			{
				current.firstSelectedGameObject = o;
				if (current.currentInputModule is GamepadInputModule)
				{
					current.SetSelectedGameObject(null, null);
					current.SetSelectedGameObject(o, null);
				}
			}
		}
	}
}
