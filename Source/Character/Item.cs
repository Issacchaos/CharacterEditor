using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CharacterEditor.Character
{
	public class Item : ICharacterData
	{
		public const int AttributeCount = 32;

		public byte Type;
		public byte Subtype;
		public short Modifier;
		private int unknown1;
		public byte Rarity;
		public byte Material;	// TODO Sometimes denotes what class can use the item...
		public byte Flags;		// TODO Noted in ItemIDs.txt
		public short Level;

		public List<ItemAttribute> Attributes;

		public Item()
		{
			Attributes = new List<ItemAttribute>();
		}

		public string FriendlyName
		{
			get
			{
				try
				{
					// <Modifier> <Material> <Item Name>
					StringBuilder name = new StringBuilder();

					if (Material != 0)
					{
						name.Append(Constants.ItemMaterialNames[Material]);
						name.Append(" ");
					}

					name.Append(Constants.ItemSubtypes[Type][Subtype]);

					return name.ToString();
				}
				catch (Exception)
				{
					Console.WriteLine("FriendlyName error: Type = {0}; Subtype = {1}", Type, Subtype);
					return "ERROR";
				}
			}
		}

		public void Read(BinaryReader reader)
		{
			Type = reader.ReadByte();
			Subtype = reader.ReadByte();
			reader.Skip(2);
			Modifier = reader.ReadInt16();
			reader.Skip(2);
			unknown1 = reader.ReadInt32();
			Rarity = reader.ReadByte();
			Material = reader.ReadByte();
			Flags = reader.ReadByte();
			reader.Skip(1);
			Level = reader.ReadInt16();
			reader.Skip(2);

			for (int i = 0; i < AttributeCount; ++i)
			{
				ItemAttribute attribute = new ItemAttribute();
				attribute.Read(reader);

				Attributes.Add(attribute);
			}

			// AttributesUsed is calculated on write
			reader.Skip(4);

			// TODO Ignore recipes for now
			if (Type == 2)
				Subtype = 0;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(Type);
			writer.Write(Subtype);
			writer.Skip(2);
			writer.Write(Modifier);
			writer.Skip(2);
			writer.Write(unknown1);
			writer.Write(Rarity);
			writer.Write(Material);
			writer.Write(Flags);
			writer.Skip(1);
			writer.Write(Level);
			writer.Skip(2);

			foreach (ItemAttribute attribute in Attributes)
				attribute.Write(writer);

			writer.Write(Attributes.Count(attr => attr.Used));
		}
	}
}