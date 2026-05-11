using UnityEngine;

public static class Define
{
    public enum InventoryItemType { Rock, Spade, Star }

    public enum EState
    {
        None,
        Idle,
        Move,
        Serving_Idle,
        Serving_Move,
        Mine,
        Move_Mine,
    }

    public static int IDLE         = Animator.StringToHash("Idle");
    public static int MOVE         = Animator.StringToHash("Move");
    public static int SERVING_IDLE = Animator.StringToHash("Serving_Idle");
    public static int SERVING_MOVE = Animator.StringToHash("Serving_Move");
    public static int MINE         = Animator.StringToHash("Mine");
    public static int MOVE_MINE    = Animator.StringToHash("Move_Mine");

    public static int GUEST_IDLE = Animator.StringToHash("Guest_Idle");
    public static int GUEST_MOVE = Animator.StringToHash("Guest_Move");

    public static int CAMP_DOOR_OPEN  = Animator.StringToHash("Camp_Door_Open");
    public static int CAMP_DOOR_CLOSE = Animator.StringToHash("Camp_Door_Close");

    public static int MINE_SHOP_FIRST_OPEN  = Animator.StringToHash("MineShop_First_Open");
    public static int MINE_SHOP_FULL_AMOUNT = Animator.StringToHash("MineShop_FullAmount");

    public static int SPADE_MACHINE_RUN = Animator.StringToHash("Run");

    public static int TUTORIAL_ARROW_IDLE = Animator.StringToHash("Tutorial_Arrow_Idle");

    public enum EGuestState
    {
        None,
        Idle,
        Move,
    }

    public enum ResourceType
    {
        Star,
        Wood,
        Rock,
        Gold,
    }

    public enum WorkerType
    {
        Mining,
        Counter,
    }

    public enum SoundType
    {
        GetStar,
        GuestGetItem,
        ItemPutDown,
        Mine1,
        Mine2,
        OpenAD,
        PotDownRock,
        GetOnSpade,
        Purchase,
        MachineSound,
    }
}
