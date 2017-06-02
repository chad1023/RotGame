using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PrefabSetting
{
    public GameObject Prefab;
    public bool IsAutoGenerate = true;
    public int initailSize;
    public string PrefabName
    {
        get
        {
            return Prefab.name;
        }
    }
    public string PoolName;
}



public class JObjectPool : MonoBehaviour
{


    public static JObjectPool _InstanceJObjectPool = null;

    public bool IsDontdestroyonload;
    [Tooltip("if true will Show Debug")]
    public bool DebugMode;
    [Tooltip("if true will delete 'CLONE' in all prefabs' name")]
    public bool HideClone;

    public PrefabSetting[] m_PrefabSetting;

    public Dictionary<string, List<GameObject>> m_Dictionary;
    public Dictionary<string, GameObject> m_DictionaryForParent;
    private void Awake()
    {
        if (IsDontdestroyonload)
            DontDestroyOnLoad(this.gameObject);

        //Check if there is already an instance of JObjectPool
        if (_InstanceJObjectPool == null)
            _InstanceJObjectPool = this;
        //If _InstanceJObjectPool already exists ,Destroy this 
        //this enforces our singleton pattern so there can only be one instance of JObjectPool.
        else if (_InstanceJObjectPool != this)
            Destroy(this.gameObject);

        m_Dictionary = new Dictionary<string, List<GameObject>>();
        m_DictionaryForParent = new Dictionary<string, GameObject>();
        Init();
    }

    #region private function

    /// <summary>
    /// Instantiate Gameobject in m_PrefabSetting into JObject pool if IsAutoGenerate is true
    /// </summary>
    /// <param name="s"></param>
    void Init()
    {
        ShowDebug("Start Initialize JObjectPool");
        for (int i = 0; i < m_PrefabSetting.Length; i++)
        {
            if (m_PrefabSetting[i].IsAutoGenerate)
            {
                GameObject tempParent = new GameObject();
                tempParent.name = m_PrefabSetting[i].PoolName;
                List<GameObject> m_list = new List<GameObject>();
                for (int j = 0; j < m_PrefabSetting[i].initailSize; j++)
                {
                    GameObject temp = (GameObject)Instantiate(m_PrefabSetting[i].Prefab, tempParent.transform);
                    temp.SetActive(false);
                    if (HideClone)
                        temp.name = m_PrefabSetting[i].PrefabName;
                    m_list.Add(temp);
                }
                m_Dictionary.Add(m_PrefabSetting[i].PrefabName, m_list);
                m_DictionaryForParent.Add(m_PrefabSetting[i].PrefabName, tempParent);
            }
        }
        ShowDebug("Finish Initialize JObjectPool");
    }

    /// <summary>
    /// Debug something if DebugMode is true
    /// </summary>
    /// <param name="s"></param>
    void ShowDebug(string s)
    {
        if (DebugMode)
            Debug.Log(s);
    }

    /// <summary>
    /// Debug Error something if DebugMode is true
    /// </summary>
    /// <param name="s"></param>
    void ShowError(string s)
    {
        if (DebugMode)
            Debug.LogError("[ " + s + " ]");

    }
    #endregion

    #region public function
    /// <summary>
    /// Get gameobject in JObject pool
    /// </summary>
    /// <param name="s"></param>
    public GameObject GetGameObject(string s, Vector3 pos, Quaternion qua)
    {
        //尋找物件池裡有沒有這個名字的物件
        if (m_Dictionary.ContainsKey(s))
        {
            for (int i = 0; i < m_Dictionary[s].Count; i++)
            {
                if (!m_Dictionary[s][i].activeSelf)
                {
                    m_Dictionary[s][i].transform.position = pos;
                    m_Dictionary[s][i].transform.rotation = qua;
                    m_Dictionary[s][i].SetActive(true);
                    return m_Dictionary[s][i];
                }
            }
            //如果都被使用的話，新創10個物件進入物件池。最後回傳最後一個物件
            ShowDebug("Ur JObject pool is all uesd , now instantiate new 10 objects");
            for (int i = 0; i < 9; i++)
            {
                GameObject temp = (GameObject)Instantiate(m_Dictionary[s][0], m_DictionaryForParent[s].transform);
                temp.SetActive(false);
                if (HideClone)
                    temp.name = m_Dictionary[s][0].name;
                m_Dictionary[s].Add(temp);
            }
            GameObject temp1 = (GameObject)Instantiate(m_Dictionary[s][0], m_DictionaryForParent[s].transform);
            temp1.transform.position = pos;
            temp1.transform.rotation = qua;
            temp1.SetActive(true);
            if (HideClone)
                temp1.name = m_Dictionary[s][0].name;
            m_Dictionary[s].Add(temp1);
            return temp1;
        }
        //如果沒有這個物件在物件池，就回傳null
        else
        {
            ShowError(s + " doesn't exist in JObject pool");
            return null;
        }
    }
    /// <summary>
    /// Get gameobject in JObject pool
    /// </summary>
    /// <param name="s"></param>
    public GameObject GetGameObject(string s, Vector3 pos)
    {
        return GetGameObject(s, pos, Quaternion.identity);
    }
    

    /// <summary>
    /// Recovery Gameobject
    /// </summary>
    /// <param name="s"></param>
    public void Recovery(GameObject g)
    {
        Recovery(g, Vector3.zero);
    }
    /// <summary>
	/// Recovery Gameobject and set its position
	/// </summary>
	/// <param name="s"></param>
    public void Recovery(GameObject g, Vector3 pos)
    {
        g.SetActive(false);
        g.transform.position = pos;
        string s = g.name;
        if (s.Contains("Clone"))
        {
            s = s.Replace("(Clone)", "");
        }
        if (m_DictionaryForParent.ContainsKey(s))
            g.transform.SetParent(m_DictionaryForParent[s].transform);
    }

    /// <summary>
    /// Delay f seconds than recovery object
    /// </summary>
    /// <param name="s"></param>
    public IEnumerator DelayRecovery(GameObject g,float f){

        yield return new WaitForSeconds(f);
        Recovery(g);
    }
    /// <summary>
	/// Instantiate Objects into JObject pool , the parameter is the prefab's name 
	/// </summary>
	/// <param name="s"></param>
    public void InstantiateObject(string s)
    {
        int _array = -1;
        for (int i = 0; i < m_PrefabSetting.Length; i++)
        {
            if (m_PrefabSetting[i].PrefabName == s)
                _array = i;
        }
        if (_array < 0)
            return;

        if (m_Dictionary.ContainsKey(s))
        {
            for (int j = 0; j < m_PrefabSetting[_array].initailSize; j++)
            {
                GameObject temp = (GameObject)Instantiate(m_PrefabSetting[_array].Prefab, m_DictionaryForParent[s].transform);
                temp.SetActive(false);
                if (HideClone)
                    temp.name = m_PrefabSetting[_array].PrefabName;
                m_Dictionary[s].Add(temp);
            }
        }
        else
        {
            GameObject tempParent = new GameObject();
            tempParent.name = m_PrefabSetting[_array].PoolName;
            List<GameObject> m_list = new List<GameObject>();
            for (int j = 0; j < m_PrefabSetting[_array].initailSize; j++)
            {
                GameObject temp = (GameObject)Instantiate(m_PrefabSetting[_array].Prefab, tempParent.transform);
                temp.SetActive(false);
                if (HideClone)
                    temp.name = m_PrefabSetting[_array].PrefabName;
                m_list.Add(temp);
            }
            m_Dictionary.Add(m_PrefabSetting[_array].PrefabName, m_list);
            m_DictionaryForParent.Add(m_PrefabSetting[_array].PrefabName, tempParent);
        }
    }
    #endregion


}