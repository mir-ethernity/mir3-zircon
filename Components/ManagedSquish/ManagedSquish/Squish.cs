using NativeSquish_x64;
using NativeSquish_x86;
using System;
using System.Runtime.InteropServices;

namespace ManagedSquish
{
	public static class Squish
	{
		private delegate int GetStorageRequirementsDelegate(int width, int height, int flags);

		private delegate void CompressFunctionDelegate(IntPtr rgba, IntPtr block, int flags);

		private delegate void CompressMaskedDelegate(IntPtr rgba, int mask, IntPtr block, int flags);

		private delegate void DecompressDelegate(IntPtr rgba, IntPtr block, int flags);

		private delegate void CompressImageDelegate(IntPtr rgba, int width, int height, IntPtr blocks, int flags);

		private delegate void DecompressImageDelegate(IntPtr rgba, int width, int height, IntPtr blocks, int flags);

		private static GetStorageRequirementsDelegate GetStorageRequirementsFunction;

		private static CompressFunctionDelegate CompressFunction;

		private static CompressMaskedDelegate CompressMaskedFunction;

		private static DecompressDelegate DecompressFunction;

		private static CompressImageDelegate CompressImageFunction;

		private static DecompressImageDelegate DecompressImageFunction;

		static Squish()
		{
			if (IntPtr.Size == 8)
			{
				Getx64Delegates();
			}
			else
			{
				Getx86Delegates();
			}
		}

		private static void Getx86Delegates()
		{
			GetStorageRequirementsFunction = NativeSquish_x86.Squish.GetStorageRequirements;
			CompressFunction = NativeSquish_x86.Squish.Compress;
			CompressMaskedFunction = NativeSquish_x86.Squish.CompressMasked;
			DecompressFunction = NativeSquish_x86.Squish.Decompress;
			CompressImageFunction = NativeSquish_x86.Squish.CompressImage;
			DecompressImageFunction = NativeSquish_x86.Squish.DecompressImage;
		}

		private static void Getx64Delegates()
		{
			GetStorageRequirementsFunction = NativeSquish_x64.Squish.GetStorageRequirements;
			CompressFunction = NativeSquish_x64.Squish.Compress;
			CompressMaskedFunction = NativeSquish_x64.Squish.CompressMasked;
			DecompressFunction = NativeSquish_x64.Squish.Decompress;
			CompressImageFunction = NativeSquish_x64.Squish.CompressImage;
			DecompressImageFunction = NativeSquish_x64.Squish.DecompressImage;
		}

		public static int GetStorageRequirements(int width, int height, SquishFlags flags)
		{
			return GetStorageRequirementsFunction(width, height, (int)flags);
		}

		public static void Compress(IntPtr rgba, IntPtr block, SquishFlags flags)
		{
			CompressFunction(rgba, block, (int)flags);
		}

		public static byte[] Compress(byte[] rgba, SquishFlags flags)
		{
			byte[] array = new byte[GetStorageRequirements(4, 4, flags)];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			GCHandle gCHandle2 = GCHandle.Alloc(rgba, GCHandleType.Pinned);
			CompressFunction(gCHandle2.AddrOfPinnedObject(), gCHandle.AddrOfPinnedObject(), (int)flags);
			gCHandle2.Free();
			gCHandle.Free();
			return array;
		}

		public static void CompressMasked(IntPtr rgba, int mask, IntPtr block, SquishFlags flags)
		{
			CompressMaskedFunction(rgba, mask, block, (int)flags);
		}

		public static byte[] CompressMasked(byte[] rgba, int mask, SquishFlags flags)
		{
			byte[] array = new byte[GetStorageRequirements(4, 4, flags)];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			GCHandle gCHandle2 = GCHandle.Alloc(rgba, GCHandleType.Pinned);
			CompressMaskedFunction(gCHandle2.AddrOfPinnedObject(), mask, gCHandle.AddrOfPinnedObject(), (int)flags);
			gCHandle2.Free();
			gCHandle.Free();
			return array;
		}

		public static void Decompress(IntPtr rgba, IntPtr block, SquishFlags flags)
		{
			DecompressFunction(rgba, block, (int)flags);
		}

		public static byte[] Decompress(byte[] block, SquishFlags flags)
		{
			byte[] array = new byte[64];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			GCHandle gCHandle2 = GCHandle.Alloc(block, GCHandleType.Pinned);
			DecompressFunction(gCHandle.AddrOfPinnedObject(), gCHandle2.AddrOfPinnedObject(), (int)flags);
			gCHandle2.Free();
			gCHandle.Free();
			return array;
		}

		public static void CompressImage(IntPtr rgba, int width, int height, IntPtr blocks, SquishFlags flags)
		{
			CompressImageFunction(rgba, width, height, blocks, (int)flags);
		}

		public static byte[] CompressImage(byte[] rgba, int width, int height, SquishFlags flags)
		{
			byte[] array = new byte[GetStorageRequirements(4, 4, flags)];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			GCHandle gCHandle2 = GCHandle.Alloc(rgba, GCHandleType.Pinned);
			CompressImageFunction(gCHandle2.AddrOfPinnedObject(), width, height, gCHandle.AddrOfPinnedObject(), (int)flags);
			gCHandle2.Free();
			gCHandle.Free();
			return array;
		}

		public static void DecompressImage(IntPtr rgba, int width, int height, IntPtr blocks, SquishFlags flags)
		{
			DecompressImageFunction(rgba, width, height, blocks, (int)flags);
		}

		public static byte[] DecompressImage(byte[] blocks, int width, int height, SquishFlags flags)
		{
			byte[] array = new byte[width * height * 4];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			GCHandle gCHandle2 = GCHandle.Alloc(blocks, GCHandleType.Pinned);
			DecompressImageFunction(gCHandle.AddrOfPinnedObject(), width, height, gCHandle2.AddrOfPinnedObject(), (int)flags);
			gCHandle2.Free();
			gCHandle.Free();
			return array;
		}
	}
}
