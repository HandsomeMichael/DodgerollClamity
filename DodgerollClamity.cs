using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgerollClamity.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DodgerollClamity
{
	public class DodgerollClamity : Mod
	{
		public static DodgerollClamity Get => ModContent.GetInstance<DodgerollClamity>();
		public Mod calamity = null;

		public override void PostSetupContent()
		{
			if (!ModLoader.TryGetMod("CalamityMod", out calamity))
			{
				Logger.Warn("The humble ram usage imploder is not enabled");
			}
        }

		// current only does dodge, later i would problaby add parry , cancel dodge, cancel dash, cancel item use , throw weapon 
		internal enum MessageType : byte
		{
			FuckingDodge,
			FuckingDodgeServer,
			InstinctDodged,
			InstinctDodgedServer
		}
		
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType msgType = (MessageType)reader.ReadByte();

			switch (msgType)
			{
				// server be like : hmm i should send all ts to other client except for you again
				case MessageType.FuckingDodgeServer:
					if (whoAmI != 255)
					{
						ModPacket modPacket = GetPacket();
						modPacket.Write((byte)MessageType.FuckingDodge);
						modPacket.Write((byte)whoAmI);
						modPacket.WriteVector2(reader.ReadVector2());
						modPacket.Write(reader.ReadInt16());
						modPacket.Send(-1, whoAmI);
					}
					else
					{
						Logger.WarnFormat("Dodgeroll: packet shouldve not been sent to a client, wtf", msgType);
					}
					break;
				// client be like : uhhhh. okay
				case MessageType.FuckingDodge:
				
					byte playerNumber = reader.ReadByte();
					var boost = reader.ReadVector2();
					int direction = reader.ReadInt16();
					Main.player[playerNumber].GetModPlayer<DodgerollPlayer>().InitiateDodgeroll(boost, direction);
					
					break;

				//sync instinct dodge
				case MessageType.InstinctDodgedServer:
					if (whoAmI != 255)
					{
						ModPacket modPacket = GetPacket();
						modPacket.Write((byte)MessageType.FuckingDodge);
						modPacket.Write((byte)whoAmI);
						modPacket.Send(-1, whoAmI);
					}
					else
					{
						Logger.WarnFormat("Dodgeroll: packet shouldve not been sent to a client, wtf", msgType);
					}
					break;

				case MessageType.InstinctDodged:
					int pNum = reader.ReadByte();
					if (Main.player[pNum].TryGetModPlayer(out DodgerollPlayer dp)) dp.InstinctDodged();
					break;
				default:
					Logger.WarnFormat("Dodgeroll: Unknown Message type: {0}", msgType);
					break;
			}
		}
	}
}
