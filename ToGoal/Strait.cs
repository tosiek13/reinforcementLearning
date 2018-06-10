using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strait : Agent
{
	[Header("Specific to Ball3D")]
	public GameObject ball;
	public GameObject platform;
	public GameObject goal;
	public bool player = false;

	public override void InitializeAgent()
	{
		resetBall ();
		resetGoal();
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "redGoal" || collision.gameObject.tag == "greenGoal")
		{
			//ADD REWARD
			resetBall();
			resetGoal();
			AddReward (1.0f);
		}
	}

	public override void CollectObservations()
	{
		AddVectorObs ((ball.transform.position - goal.transform.position));
		AddVectorObs(ball.transform.GetComponent<Rigidbody>().velocity);
	}



	public override void AgentAction(float[] vectorAction, string textAction)
	{
		if (player) {
			float speed_factor = 1f;
			Vector3 newSpeed = ball.GetComponent<Rigidbody> ().velocity + new Vector3 (speed_factor * vectorAction[0], 0.0f, speed_factor * vectorAction[1]);
			ball.GetComponent<Rigidbody> ().velocity = newSpeed;
		} else {
			float speed_factor = 1f;
			float action_x = speed_factor * Mathf.Clamp(vectorAction[0], -1f, 1f);
			float action_z = speed_factor * Mathf.Clamp(vectorAction[1], -1f, 1f);
			Vector3 newSpeed = ball.GetComponent<Rigidbody> ().velocity + new Vector3 (action_x, 0.0f, action_z);
			ball.GetComponent<Rigidbody> ().velocity = newSpeed;
			AddReward(-0.1f);
		}
		if(outOfBoard()){
			Done ();
			AddReward(-1f);
		}
	}

	public override void AgentReset()
	{
		resetBall ();
		resetGoal();
	}

	public void resetBall(){
		ball.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; 
		ball.transform.position = new Vector3(-platform.GetComponent<Collider>().bounds.size.x / 2 + platform.transform.position.x + 1.0f, 2.0f , 0.0f);
		//ball.transform.position = new Vector3(Random.Range(-0.5f, 0.5f), 0.1f, Random.Range(-0.5f, 0.5f)) + platform.transform.position;
	}

	public bool outOfBoard(){
		return ball.transform.position.y - platform.transform.position.y < 0;
	}

	public void resetGoal(){
		float range = platform.GetComponent<Collider> ().bounds.size.z / 2;
		float new_z = Random.Range (-range, range);
		goal.transform.position = new Vector3 (goal.transform.position.x, goal.transform.position.y, new_z);
	}
}