using UnityEngine;
 
public static class Define
{
    public enum EState
    {
        None,
        Idle,
        Move,
        Serving_Idle,  // 아이템 소지 + 정지
        Serving_Move,  // 아이템 소지 + 이동
        Mine,          // 제자리 채굴
        Move_Mine,     // 이동 채굴
    }
 
    // 애니메이터 파라미터 해시
    public static int IDLE       = Animator.StringToHash("Idle");
    public static int MOVE       = Animator.StringToHash("Move");
    public static int SERVING_IDLE = Animator.StringToHash("Serving_Idle");
    public static int SERVING_MOVE = Animator.StringToHash("Serving_Move");
    public static int MINE       = Animator.StringToHash("Mine");        // 채굴 Idle
    public static int MOVE_MINE  = Animator.StringToHash("Move_Mine");   // 채굴 Move

    // Guest 전용 애니메이션
    public static int GUEST_IDLE = Animator.StringToHash("Guest_Idle");
    public static int GUEST_MOVE = Animator.StringToHash("Guest_Move");

    public enum EGuestState
    {
        None,
        Idle,   // 대기 중
        Move,   // 이동 중
    }
}