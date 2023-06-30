using System;

namespace UnityEngine.Purchasing.Security
{
	public class AppleTangle
	{
		private static byte[] data = Convert.FromBase64String("0F4U0YifJMoikbZL4iT5bZiDmiPCycblSYdJOMLOzsrKz8xNzs7Pk++Mjv9Nzu3/wsnG5UmHSTjCzs7Oy8nczZqc/tz/3snMmsvF3MWOv7/g/04Mycfkyc7KysjNzf9OedVOfKhAx3vvOARj4++gv3nwzv9DeIwAFvmwDkiaFmhWdv2NNBcavlGxbp2dqqOmrqGsqu+goe+7p6a876yqven/68nMmsvE3NKOv7+jqu+Mqr27cTu8VCEdq8AEtoD7F23xNrcwpAdaUbXDa4hElBvZ+PwEC8CCAdumHtBKTErUVvKI+D1mVI9B4xt+X90XZGy+XYicmg5g4I58NzQsvwIpbIN41HJcjevd5QjA0nmCU5GsB4RP2Pr9/vv//PmV2ML8+v/9//b9/vv/yCOy9kxEnO8c9wt+cFWAxaQw5DMG1r06ksEasJBUPerMdZpAgpLCPmcTse36BeoaFsAZpBtt6+zeOG5jtu+uvLy6oqq8766srKq/u66hrKpNzs/JxuVJh0k4rKvKzv9OPf/lyYYXuVD826puuFsG4s3Mzs/ObE3OoavvrKChq6a7pqChvO+gqe+6vKry6ajvRfylOMJNABEkbOA2nKWUq+PvrKq9u6appqyuu6rvv6Cjpqy2/03LdP9NzGxvzM3Ozc3Ozf/CycZP2+QfpohbucYxO6RC4Y9pOIiCsO+uoavvrKq9u6appqyuu6agoe+/uLjhrr+/o6rhrKCi4K6/v6OqrK5AvE6vCdSUxuBdfTeLhz+v91HaOsBS8jzkhufVBzEBenbBFpHTGQTylmjKxrPYj5ne0bsceETs9IhsGqAPrPy4OPXI45kkFcDuwRV1vNaAevlWg+K3eCJDVBM8uFQ9uR24/4AOq/rs2oTaltJ8Wzg5U1EAn3UOl5/H5MnOysrIzc7Z0ae7u7+89eDguLumqaasrruq7622766htu+/rr27ys/MTc7Az/9NzsXNTc7OzyteZsamqaasrrumoKHvjrq7p6C9pru2/uGPaTiIgrDHkf/Qycya0uzL1//Zv6Oq74yqvbumqaasrrumoKHvjrrvoKnvu6eq77unqqHvrr+/o6asrv/eycyay8XcxY6/v6Oq74ahrOH+fv+XI5XL/UOnfEDSEaq8MKiRqnPrLSQeeL8QwIou6AU+orciKHrY2Nn/28nMmsvM3MKOv7+jqu+doKC7o6rvhqGs4f7p/+vJzJrLxNzSjr/8+ZX/rf7E/8bJzJrLydzNmpz+3L+jqu+doKC774yO/9HYwv/5//v95UmHSTjCzs7Kys//rf7E/8bJzJq1/03Ouf/Bycya0sDOzjDLy8zNzrCOZ1c2HgWpU+uk3h9sdCvU5QzQevViO8DBz13Efu7Z4bsa88IUrdmKsdCDpJ9ZjkYLu63E30yOSPxFTrunoL2mu7b+2f/bycyay8zcwo6/va6su6asqu+8u667qqKqobu84f/J/8DJzJrS3M7OMMvK/8zOzjD/0kTWRhE2hKM6yGTt/80n1/E3n8Ycx5H/Tc7eycya0u/LTc7H/03Oy//JzJrSwcvZy9vkH6aIW7nGMTukQq2jqu+8u66hq669q++7qr2ivO+un2VFGhUrMx/GyPh/urru");

		private static int[] order = new int[61]
		{
			15,
			47,
			13,
			13,
			37,
			21,
			58,
			38,
			10,
			50,
			18,
			49,
			24,
			59,
			38,
			51,
			19,
			54,
			40,
			34,
			27,
			43,
			56,
			44,
			51,
			32,
			45,
			36,
			55,
			56,
			52,
			57,
			48,
			36,
			35,
			39,
			38,
			49,
			57,
			49,
			41,
			50,
			45,
			53,
			51,
			59,
			50,
			58,
			58,
			55,
			54,
			51,
			58,
			53,
			54,
			56,
			56,
			57,
			58,
			59,
			60
		};

		private static int key = 207;

		public static readonly bool IsPopulated = true;

		public static byte[] Data()
		{
			if (!IsPopulated)
			{
				return null;
			}
			return Obfuscator.DeObfuscate(data, order, key);
		}
	}
}
