using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgerollClamity.Content;
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
			InstinctDodged
		}
		
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType msgType = (MessageType)reader.ReadByte();

			switch (msgType)
			{
				// sync dodge
				case MessageType.FuckingDodge:
					byte playerNumber = reader.ReadByte();
					if (Main.player[playerNumber].TryGetModPlayer(out DodgerollPlayer dodgerollPlayer))
					{
						dodgerollPlayer.HandleFuckingDodge(reader, whoAmI);
					}
					break;
				// sync instinct dodge
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
