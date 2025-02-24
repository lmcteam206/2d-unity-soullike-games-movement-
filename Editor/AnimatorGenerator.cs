using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AnimatorGenerator : MonoBehaviour
{
    [MenuItem("Tools/Generate Player Animator")]
    static void GenerateAnimator()
    {
        string path = "Assets/PlayerAnimator.controller";
        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(path);

        // Parameters
        animatorController.AddParameter("Attack1", AnimatorControllerParameterType.Trigger);
        animatorController.AddParameter("Attack2", AnimatorControllerParameterType.Trigger);
        animatorController.AddParameter("Attack3", AnimatorControllerParameterType.Trigger);
        animatorController.AddParameter("Charging", AnimatorControllerParameterType.Bool);
        animatorController.AddParameter("ChargedAttack", AnimatorControllerParameterType.Trigger);
        animatorController.AddParameter("RangedAttack", AnimatorControllerParameterType.Trigger);

        // Create states (you will assign animations later)
        AnimatorStateMachine rootStateMachine = animatorController.layers[0].stateMachine;
        AnimatorState idleState = rootStateMachine.AddState("Idle");
        AnimatorState attack1State = rootStateMachine.AddState("Attack1");
        AnimatorState attack2State = rootStateMachine.AddState("Attack2");
        AnimatorState attack3State = rootStateMachine.AddState("Attack3");
        AnimatorState chargingState = rootStateMachine.AddState("Charging");
        AnimatorState chargedAttackState = rootStateMachine.AddState("ChargedAttack");
        AnimatorState rangedAttackState = rootStateMachine.AddState("RangedAttack");

        rootStateMachine.defaultState = idleState;

        // Transitions for Combo
        var t1 = idleState.AddTransition(attack1State);
        t1.AddCondition(AnimatorConditionMode.If, 0, "Attack1");
        t1.hasExitTime = false;

        var t2 = attack1State.AddTransition(attack2State);
        t2.AddCondition(AnimatorConditionMode.If, 0, "Attack2");
        t2.hasExitTime = false;

        var t3 = attack2State.AddTransition(attack3State);
        t3.AddCondition(AnimatorConditionMode.If, 0, "Attack3");
        t3.hasExitTime = false;

        // Charging to ChargedAttack
        var chargingTransition = idleState.AddTransition(chargingState);
        chargingTransition.AddCondition(AnimatorConditionMode.If, 0, "Charging");
        chargingTransition.hasExitTime = false;

        var chargedAttackTransition = chargingState.AddTransition(chargedAttackState);
        chargedAttackTransition.AddCondition(AnimatorConditionMode.If, 0, "ChargedAttack");
        chargedAttackTransition.hasExitTime = false;

        // Ranged Attack
        var rangedTransition = idleState.AddTransition(rangedAttackState);
        rangedTransition.AddCondition(AnimatorConditionMode.If, 0, "RangedAttack");
        rangedTransition.hasExitTime = false;

        Debug.Log("✅ Player Animator Controller generated at: " + path);
    }
}
