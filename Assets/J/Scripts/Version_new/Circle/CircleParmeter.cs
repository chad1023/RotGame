using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleParmeter : MonoBehaviour
{
    public GameMain gamemain;
    public int LimitNumber;
    public float Radius;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnMouseDown()
    {
        
        if (LimitNumber > 0 && gamemain.shoot != "" && gamemain.shoot != null)
        {
            // get the ray point
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D rayhit = Physics2D.Raycast(ray.origin, ray.direction, 10);
            // change it as vector3
            Vector3 modifyvecter = new Vector3(rayhit.point.x, rayhit.point.y, 0);
            modifyvecter.Normalize();
            // modify its magnitude to Radius
            modifyvecter *= Radius;

            GameObject InsLight = JObjectPool._InstanceJObjectPool.GetGameObject(gamemain.shoot, transform.position);
            InsLight.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            InsLight.transform.SetParent(this.transform);
//            Collider col = InsLight.GetComponent<Collider>();
//            col.enabled = false;
            Tweener tweener = InsLight.transform.DOMove(modifyvecter, 1f);
            tweener.SetEase(Ease.OutQuad);
//            tweener.OnComplete(() => { col.enabled = true; });
            gamemain.EnergyShoot();

            LimitNumber--;
        }
        else
        {

            Ray ray1 = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit2D rayhit1=Physics2D.Raycast (ray1.origin,ray1.direction,100);
        GameObject tempclick=JObjectPool._InstanceJObjectPool.GetGameObject("ClickEffect",rayhit1.point);
        StartCoroutine(delay(tempclick));

        }
    }

IEnumerator delay(GameObject g){
		yield return new WaitForSeconds(2f);
		JObjectPool._InstanceJObjectPool.Recovery(g);
	}
}
