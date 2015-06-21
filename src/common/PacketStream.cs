// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common.Utilities;

namespace common
{
	public class PacketStream : MemoryStream
	{
		MemoryStream inner;

		public PacketStream()
		{
			this.inner = new MemoryStream();
		}

		public PacketStream(Int16 PacketId)
		{
			this.inner = new MemoryStream();
			this.inner.Write(new byte[4], 0, 4);
			this.inner.Write(BitConverter.GetBytes(PacketId), 0, 2);
			this.inner.Write(new byte[1], 0, 1);
		}

		public override byte[] ToArray()
		{
			return this.inner.ToArray();
		}

		public PacketStream(byte[] pInner)
		{
			this.inner = new MemoryStream(pInner);
		}

		public PacketStream(MemoryStream pInner)
		{
			this.inner = pInner;
		}

		#region Write Methods
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.inner.Write(buffer, offset, count);
		}

		public void WriteInt16(Int16 val)
		{
			this.inner.Write(BitConverter.GetBytes(val), 0, 2);
		}

		public void WriteUInt16(UInt16 val)
		{
			this.inner.Write(BitConverter.GetBytes(val), 0, 2);
		}

		public void WriteInt32(Int32 val)
		{
			this.inner.Write(BitConverter.GetBytes(val), 0, 4);
		}

		public void WriteUInt32(UInt32 val)
		{
			this.inner.Write(BitConverter.GetBytes(val), 0, 4);
		}

		public void WriteInt64(Int64 val)
		{
			this.inner.Write(BitConverter.GetBytes(val), 0, 8);
		}

		public void WriteUInt64(UInt64 val)
		{
			this.inner.Write(BitConverter.GetBytes(val), 0, 8);
		}

		public void WriteFloat(Single val)
		{
			this.inner.Write(BitConverter.GetBytes(val), 0, 4);
		}

		public override void WriteByte(byte value)
		{
			this.inner.WriteByte(value);
		}

		public void WriteString(String val, int size)
		{
			byte[] tmp = Encoding.ASCII.GetBytes(val);
			this.inner.Write(tmp, 0, tmp.Length);
			this.inner.Write(new byte[size - tmp.Length], 0, size - tmp.Length);
		}

		internal void WriteString(String value)
		{
			this.WriteString(value, value.Length);
		}

		public void WriteBool(bool val)
		{
			this.inner.WriteByte((byte)(val ? 1 : 0));
		}
		#endregion

		#region Read Methods
		public override int Read(byte[] buffer, int offset, int count)
		{
			var result = this.inner.Read(buffer, offset, count);

			return result;
		}

		public byte[] ReadBytes(int offset, int count, bool countHeader = false)
		{
			if (countHeader)
				inner.Seek(offset, SeekOrigin.Begin);
			else
				inner.Seek(offset + Globals.HeaderLength, SeekOrigin.Begin);

			byte[] buffer = new byte[count];
			inner.Read(buffer, 0, count);
			return buffer;
		}

		public Int16 ReadInt16(int offset, bool countHeader = false)
		{
			if (countHeader)
				inner.Seek(offset, SeekOrigin.Begin);
			else
				inner.Seek(offset + Globals.HeaderLength, SeekOrigin.Begin);

			byte[] buffer = new byte[2];
			inner.Read(buffer, 0, 2);
			return BitConverter.ToInt16(buffer, 0);
		}

		public UInt16 ReadUInt16(int offset, bool countHeader = false)
		{
			if (countHeader)
				inner.Seek(offset, SeekOrigin.Begin);
			else
				inner.Seek(offset + Globals.HeaderLength, SeekOrigin.Begin);

			byte[] buffer = new byte[2];
			inner.Read(buffer, 0, 2);
			return BitConverter.ToUInt16(buffer, 0);
		}

		public Int32 ReadInt32(int offset, bool countHeader = false)
		{
			if (countHeader)
				inner.Seek(offset, SeekOrigin.Begin);
			else
				inner.Seek(offset + Globals.HeaderLength, SeekOrigin.Begin);

			byte[] buffer = new byte[4];
			inner.Read(buffer, 0, 4);
			return BitConverter.ToInt32(buffer, 0);
		}

		public UInt32 ReadUInt32(int offset, bool countHeader = false)
		{
			if (countHeader)
				inner.Seek(offset, SeekOrigin.Begin);
			else
				inner.Seek(offset + Globals.HeaderLength, SeekOrigin.Begin);

			byte[] buffer = new byte[4];
			inner.Read(buffer, 0, 4);
			return BitConverter.ToUInt32(buffer, 0);
		}

		public Int64 ReadInt64(int offset, bool countHeader = false)
		{
			if (countHeader)
				inner.Seek(offset, SeekOrigin.Begin);
			else
				inner.Seek(offset + Globals.HeaderLength, SeekOrigin.Begin);

			byte[] buffer = new byte[8];
			inner.Read(buffer, 0, 8);
			return BitConverter.ToInt64(buffer, 0);
		}

		public UInt64 ReadUInt64(int offset, bool countHeader = false)
		{
			if (countHeader)
				inner.Seek(offset, SeekOrigin.Begin);
			else
				inner.Seek(offset + Globals.HeaderLength, SeekOrigin.Begin);

			byte[] buffer = new byte[8];
			inner.Read(buffer, 0, 8);
			return BitConverter.ToUInt64(buffer, 0);
		}

		public Single ReadFloat(int offset, bool countHeader = false)
		{
			if (countHeader)
				inner.Seek(offset, SeekOrigin.Begin);
			else
				inner.Seek(offset + Globals.HeaderLength, SeekOrigin.Begin);

			byte[] buffer = new byte[4];
			inner.Read(buffer, 0, 4);
			return BitConverter.ToSingle(buffer, 0);
		}

		public String ReadString(int offset, int size, bool countHeader = false)
		{
			if (countHeader)
				inner.Seek(offset, SeekOrigin.Begin);
			else
				inner.Seek(offset + Globals.HeaderLength, SeekOrigin.Begin);

			byte[] buffer = new byte[size];
			inner.Read(buffer, 0, size);

			return ByteUtils.toString(buffer);
		}

		public bool ReadBool(int offset, bool countHeader = false)
		{
			if (countHeader)
				inner.Seek(offset, SeekOrigin.Begin);
			else
				inner.Seek(offset + Globals.HeaderLength, SeekOrigin.Begin);

			return (inner.ReadByte() == 0 ? false : true);
		}

		internal byte ReadByte(short offset, bool countHeader = false)
		{
			if (countHeader)
				inner.Seek(offset, SeekOrigin.Begin);
			else
				inner.Seek(offset + Globals.HeaderLength, SeekOrigin.Begin);

			return (byte)inner.ReadByte();
		}
		#endregion

		public Int32 GetSize()
		{
			inner.Seek(0, SeekOrigin.Begin);
			byte[] res = new byte[4];
			inner.Read(res, 0, 4);
			return BitConverter.ToInt32(res, 0);
		}

		public Int16 GetId()
		{
			inner.Seek(4, SeekOrigin.Begin);
			byte[] res = new byte[2];
			inner.Read(res, 0, 2);
			return BitConverter.ToInt16(res, 0);
		}

		public byte[] GetData()
		{
			byte[] res = new byte[inner.Length - 7];
			inner.Seek(7, SeekOrigin.Begin);
			inner.Read(res, 0, res.Length);

			return res;
		}

		public void SetId(Int16 pPacketId)
		{
			inner.Seek(4, SeekOrigin.Begin);
			inner.Write(BitConverter.GetBytes(pPacketId), 0, 2);
		}

		public MemoryStream GetPacket()
		{
			this.inner.Seek(0, SeekOrigin.Begin);
			this.inner.Write(BitConverter.GetBytes(inner.ToArray().Length), 0, 4);
			return this.inner;
		}
	}
}
