using UGXP.Core;
using UGXP.Core.Components;

namespace UGXP.Test;
public class ComponentTest : Component
{
    private float moveSpeed = 3f;
    void Update() {
        //Debug.Log(Time.deltaTime);

        Debug.Log(transform.position);

        this.transform.position += moveSpeed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision2D col) {
        Debug.Log("enter");
    }

    void OnCollisionStay(Collision2D col) {
        Debug.Log("stay");
    }

    void OnCollisionExit(Collision2D col) {
        Debug.Log("exit");
    }
}
