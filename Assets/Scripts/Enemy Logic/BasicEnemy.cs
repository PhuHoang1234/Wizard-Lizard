using UnityEngine;

public class BasicEnemy : EnemyBase
{
    protected override void Start()
    {
        base.Start();
        visionDistance = 15f;
        visionAngle = 39.2f;
        hearingDistance = 0f;
    }

    protected override void Update()
    {
        base.Update();
        Debug.Log(state);
    }
}
