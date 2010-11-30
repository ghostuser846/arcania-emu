using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public partial class Systems
    {
        public const ushort CLIENT_CHARACTERSCREEN = 0x703A,
            CLIENT_PATCH = 0x9000,
            CLIENT_CONNECTION = 0x6103,
            CLIENT_PING = 0x2001,
            CLIENT_PING2 = 0x2002,
            CLIENT_INGAME_REQUEST = 0x72EE,
            CLIENT_INGAME_SUCCESS = 0x36CF,
            CLIENT_MOVEMENT = 0x7674,
            CLIENT_LEAVE_REQUEST = 0x744E,
            CLIENT_LEAVE_CANCEL = 0x7413,
            CLIENT_ITEM_MOVE = 0x73CF,
            CLIENT_SELECT_OBJECT = 0x7445,
            CLIENT_GM = 0x726D,
            CLIENT_EMOTE = 0x33F8,
            CLIENT_TELEPORTDATA = 0x32E7,
            CLIENT_TELEPORTSTART = 0x73AF,
            CLIENT_CHAT = 0x7576,
            CLIENT_MAINACTION = 0x70F0,
            CLIENT_MASTERY_UP = 0x70A0,
            CLIENT_SKILL_UP = 0x779D,
            CLIENT_SAVE_INFO = 0x7611,
            CLIENT_GETUP = 0x34C9,
            CLIENT_REQUEST_PARTY = 0x7442,
            CLIENT_PARTY_REQUEST = 0x368B,
            CLIENT_PARTY_ADDMEMBERS = 0x74F8,
            CLIENT_PARTY_BANPLAYER = 0x7170,
            CLIENT_PARTY_LEAVE = 0x7465,
            CLIENT_JOIN_FORMED_PARTY = 0x776F,
            CLIENT_FORMED_PARTY_DELETE = 0x7010,
            CLIENT_PLAYER_UPDATE_STR = 0x71D0,
            CLIENT_PLAYER_UPDATE_INT = 0x724A,
            CLIENT_PLAYER_HANDLE = 0x754B,
            CLIENT_PLAYER_BERSERK = 0x7436,
            CLIENT_EXCHANGE_WINDOWS_CLOSE = 0x7066,
            CLIENT_EXCHANGE_REQUEST = 0x77F2,
            CLIENT_EXCHANGE_ACCEPT = 0x7207,
            CLIENT_EXCHANGE_APPROVE = 0x745C,
            CLIENT_CLOSE_NPC = 0x770C,
            CLIENT_OPEN_NPC = 0x74F2,
            CLIENT_NPC_BUYPACK = 0x70AF,
            CLIENT_OPEN_WAREHOUSE = 0x7711,
            CLIENT_CLOSE_SCROLL = 0x70F4,
            CLIENT_SAVE_PLACE = 0x7228,
            CLIENT_MOVEMENT_WITH_TRANSPORT = 0x749B,
            CLIENT_PET_TERMINATE = 0x7779,
            CLIENT_PARTYMATCHING_LIST_REQUEST = 0x77EE,
            CLIENT_CREATE_FORMED_PARTY = 0x73EE,
            CLIENT_ALCHEMY = 0x7403,
            CLIENT_STALL_OPEN = 0x7036,
            CLIENT_STALL_WELC = 0x7783;



        public const ushort SERVER_CHARACTERSCREEN = 0xB03A,
            SERVER_CONNECTION = 0xA103,
            SERVER_AGENTSERVER = 0x2001,
            SERVER_PATCH = 0x600D,
            SERVER_LOGINSCREEN_ACCEPT = 0xB2EE,
            SERVER_STARTPLAYERDATA = 0x33E8,
            SERVER_PLAYERDATA = 0x3024,
            SERVER_ENDPLAYERDATA = 0x3750,
            SERVER_PLAYERSTAT = 0x369C,
            SERVER_UNKNOWNPACK = 0x3064,
            SERVER_MOVEMENT = 0xB674,
            SERVER_LEAVE_ACCEPT = 0xB44E,
            SERVER_LEAVE_CALCEL = 0xB413,
            SERVER_LEAVE_SUCCESS = 0x360B,
            SERVER_GROUPSPAWN_START = 0x37D1,
            SERVER_GROUPSPAWN_DATA = 0x36B7,
            SERVER_GROUPSPAWN_END = 0x377E,
            SERVER_ITEM_MOVE = 0xB3CF,
            SERVER_ITEM_UN_EFFECT = 0x37D3,
            SERVER_ITEM_EFFECT = 0x3220,
            SERVER_SELECT_OBJECT = 0xB445,
            SERVER_EMOTE = 0x33F8,
            SERVER_CHANGE_STATUS = 0x3304,
            SERVER_SOLO_SPAWN = 0x35C8,
            SERVER_SOLO_DESPAWN = 0x3183,
            SERVER_CHAT = 0x326E,
            SERVER_CHAT_INDEX = 0xB576,
            SERVER_TELEPORTSTART = 0xB3AF,
            SERVER_TELEPORTOTHERSTART = 0x36A5,
            SERVER_TELEPORTIMAGE = 0x37D5,
            SERVER_SILKPACK = 0x33B6,
            SERVER_ACTIONSTATE = 0xB0F0,
            SERVER_ACTION_DATA = 0xB083,
            SERVER_SKILL_DATA = 0xB5B5,
            SERVER_SKILL_ICON = 0xB258,
            SERVER_SKILL_ENDBUFF = 0xB4C3,
            SERVER_SETSPEED = 0x365E,
            SERVER_MASTERY_UP = 0xB0A0,
            SERVER_INFO_UPDATE = 0x3213,
            SERVER_SKILL_UPDATE = 0xB79D,
            SERVER_ARROW_UPDATE = 0x304A,
            SERVER_PICKUPITEM_MOVE = 0xB477,
            SERVER_PICKUPITEM_EGILME = 0x3138,
            SERVER_UPDATEGOLD = 0x3213,
            SERVER_PARTY_REQUEST = 0x368B,
            SERVER_PARTY_UNKNOWN1 = 0xB652,
            SERVER_PARTY_UNKNOWN2 = 0xB442,
            SERVER_FORMED_PARTY_NOT_FOUND = 0xB76F,
            SERVER_LIST_FORMED_PARTY_DATA = 0x300A,
            SERVER_PARTYMEMBER_DATA = 0x300A,
            SERVER_PARTY_DATA = 0x3AEF,
            SERVER_DELETE_FORMED_PARTY = 0xB010,
            SERVER_SEND_PARTYLIST = 0xB7EE,
            SERVER_FORMED_PARTY_CREATED = 0xB3EE,
            SERVER_PLAYER_STATE_UPDATE = 0x33AE,
            SERVER_PLAYER_LEVELUP_EFFECT = 0x325E,
            SERVER_PLAYER_GET_EXP = 0x31B4,
            SERVER_PLAYER_UPDATE_STR = 0xB1D0,
            SERVER_PLAYER_UPDATE_INT = 0xB24A,
            SERVER_PLAYER_HANDLE_EFFECT = 0x34F1,
            SERVER_PLAYER_HANDLE_UPDATE_SLOT = 0xB54B,
            SERVER_UNIQUE_ANNOUNCE = 0x3588,
            SERVER_ITEM_DELETE = 0x375C,
            SERVER_EXCHANGE_WINDOW = 0x3411,
            SERVER_EXCHANGE_PROCESS = 0xB7F2,
            SERVER_EXCHANGE_CLOSE = 0xB066,
            SERVER_EXCHANGE_ITEM = 0x336F,
            SERVER_EXCHANGE_ACCEPT = 0xB207,
            SERVER_EXCHANGE_ACCEPT2 = 0x35BB,
            SERVER_EXCHANGE_GOLD = 0x37A9,
            SERVER_EXCHANGE_APPROVE = 0xB45C,
            SERVER_EXCHANGE_FINISHED = 0x33E6,
            SERVER_EXCHANGE_CANCEL = 0x3637,
            SERVER_CLOSE_NPC = 0xB70C,
            SERVER_OPEN_NPC = 0xB4F2,
            SERVER_OPEN_WAREHOUSE = 0x374F,
            SERVER_OPEN_WAREHOUSE2 = 0x36AE,
            SERVER_OPEN_WAREPROB = 0x33F2,
            SERVER_SAVE_PLACE = 0xB228,
            SERVER_PET_INFORMATION = 0x3106,
            SERVER_PLAYER_UPTOHORSE = 0xB5A5,
            SERVER_PLAYER_DESPAWN_PET = 0x34E8,
            SERVER_ITEM_QUANTITY_UPDATE = 0x32A5,
            SERVER_ALCHEMY = 0xB403,
            SERVER_STALL_OPEN = 0x332A,
            SERVER_STALL_OPENED = 0xB036,
            SERVER_STALL_ADDITEM = 0xB783;
    }
    /// <summary>
    /// Example opcode system.
    /// </summary>
    public partial class opCodes
    {
        public class Client
        {
            public struct Player
            {
                public const ushort CHARACTERSCREEN = 0xB03A;
            }
        }
        public class Server
        {

        }
    }
}
