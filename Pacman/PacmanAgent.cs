using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanAgent : Agent
{
	[Header("Specific to Ball3D")]
	public GameObject ball;
	public GameObject platform;
	public GameObject goal;

	private int GOALS_AMOUNT = 1;
	public bool player = false;
	private float platformHalfEdge;
	private List<GameObject> goals = new List<GameObject>();

	public override void InitializeAgent()
	{
		platformHalfEdge = platform.GetComponent<Collider> ().bounds.size.x / 2;
		goals.Add (goal);
		for(int i = 1; i < GOALS_AMOUNT; i++){
			goals.Add(Instantiate(goal));
		}
		reset ();
	}

	void OnCollisionEnter(Collision collision) {
		if (/*collision.gameObject.tag == "redGoal" ||*/collision.gameObject.tag == "greenGoal")
		{
			//Done();
			AddReward (1.0f);
			resetGoal (collision.gameObject);
		}
	}

	public override void CollectObservations()
	{
		AddVectorObs (goals[0].transform.position - ball.transform.position);
		AddVectorObs (ball.transform.GetComponent<Rigidbody>().velocity);
		//pozycja względem środka platformy
		/*AddVectorObs (ball.transform.position - platform.transform.position);
		AddVectorObs (platform.GetComponent<Collider> ().bounds.size.x);
		AddVectorObs (ball.transform.GetComponent<Rigidbody>().velocity);
		for (var i = 0; i < goals.Count; i++) {
			AddVectorObs (ball.transform.position - goals[i].transform.position);
		}*/
	}



	public override void AgentAction(float[] vectorAction, string textAction)
	{
		Vector3 preStepDist = goals [0].transform.position - ball.transform.position;
		if (player) {
			float speed_factor = 1f;
			Vector3 newSpeed = ball.GetComponent<Rigidbody> ().velocity + new Vector3 (speed_factor * vectorAction[0], 0.0f, speed_factor * vectorAction[1]);
			ball.GetComponent<Rigidbody> ().velocity = newSpeed;
		} else {
			float speed_factor = 1f;
			float action_x = speed_factor * Mathf.Clamp(vectorAction[0], -0.1f, 0.1f);
			float action_z = speed_factor * Mathf.Clamp(vectorAction[1], -0.1f, 0.1f);
			Vector3 newSpeed = ball.GetComponent<Rigidbody> ().velocity + new Vector3 (action_x, 0.0f, action_z);
			ball.GetComponent<Rigidbody> ().velocity = newSpeed;
			Vector3 postStepDist = goals [0].transform.position - ball.transform.position;
			AddReward((preStepDist - postStepDist).normalized.sqrMagnitude);
		}
		if(outOfBoard()){
			Done ();
			AddReward(-1f);
		}
	}

	public override void AgentReset()
	{
		reset();
	}

	public void resetBall(){
		ball.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; 
		ball.transform.position = platform.transform.position +  new Vector3 (0.0f, 1.0f, 0.0f);
	}

	public void resetGoals(){
		for (var i = 0; i < goals.Count; i++) {
			resetGoal (goals[i]);
		}
	}

	public void resetGoal(GameObject goal){
		goal.transform.position = newGoalPosition ();
	}

	private Vector3 newGoalPosition(){
		float range = platformHalfEdge;
		float new_x = Random.Range (-range, range);
		float new_z = Random.Range (-range, range);
		return platform.transform.position + new Vector3 (new_x, 0.5f, new_z);
	}

	public bool outOfBoard(){
		return ball.transform.position.y < platform.transform.position.y;
	}
		
	private void reset(){
		resetBall ();
		resetGoals();
	}
}