using UnityEngine;
public class LookAtTarget : MonoBehaviour {
  public Transform target;
  void LateUpdate(){ if (target) transform.LookAt(target.position); }
}
