using UnityEngine;
 
public static class Define
{
    public enum EState
    {
        None,
        Idle,
        Move,
        Mine,       // 제자리에서 채굴 중
        Move_Mine,  // 이동하면서 채굴 중
    }
 
    // 애니메이터 파라미터 해시
    public static int IDLE       = Animator.StringToHash("Idle");
    public static int MOVE       = Animator.StringToHash("Move");
    public static int SERVING_IDLE = Animator.StringToHash("Serving_Idle");
    public static int SERVING_MOVE = Animator.StringToHash("Serving_Move");
    public static int MINE       = Animator.StringToHash("Mine");        // 채굴 Idle 애니메이션
    public static int MOVE_MINE  = Animator.StringToHash("Move_Mine");   // 채굴 Move 애니메이션
}