using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    List<Gloob> gloobList;
    public bool currentlySelecting = false;
    private Vector2 startClick;
    private Vector2 endClick;
    public GameObject superGloob;
    public static PlayerInput Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        gloobList = new List<Gloob>();
        if(Instance == null)
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMove()
    {
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        StartCoroutine(moveEachOverTime(clickPos));
    }

    private IEnumerator moveEachOverTime(Vector3 pos)
    {
        foreach (Gloob gloob in gloobList)
        {
            Debug.Log(pos);
            gloob.Move(pos);
            yield return new WaitForSeconds(.2f);
        }
    }

    void OnSelect()
    {
        if (currentlySelecting)
        {
            List<Gloob> tempG = new List<Gloob> ();
            currentlySelecting = false;
            endClick = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            List<Collider2D> res = new List<Collider2D> ();
            Physics2D.OverlapArea(startClick, endClick,new ContactFilter2D(), res);
            foreach(Collider2D c in res)
            {
                Gloob g = c.attachedRigidbody.gameObject.GetComponent<Gloob>();
                if (g != null)
                {
                    tempG.Add(g);
                }
            }

            if(tempG.Count > 0)
            {
                gloobList = tempG;
            }
        }
        else
        {
            currentlySelecting=true;
            startClick = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
    }

    void levelEnd()
    {
        gloobList.Clear();
        currentlySelecting = false;
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene.Equals("Level 1"))
        {
            SceneController.ChangeScene("Level 2");
        }
        else if (currentScene.Equals("Level 2"))
        {
            SceneController.ChangeScene("Level 3");
        }
        else if (currentScene.Equals("Level 3"))
        {
            SceneController.ChangeScene("Level 4");
        }
        else if (currentScene.Equals("Level 4"))
        {
            SceneController.ChangeScene("Level 5");
        }
        else
        {
            SceneController.ChangeScene("Main Menu");
        }
    }

    void OnMerge()
    {
        if(gloobList.Count > 1)
        {
            int w = 0;
            Gloob first = gloobList[0];
            Gloob tempGloob = Instantiate(first, new Vector3(-10, -10, -10), Quaternion.identity);
            foreach (Transform child in tempGloob.gameObject.transform)
            {
                child.gameObject.transform.parent = null;
            }
            tempGloob.hover = false;
            Vector2Int pos = Vector2Int.FloorToInt(first.transform.position);
            
            foreach (Gloob g in gloobList)
            {
                w += g.weight;
                if (pos != Vector2Int.FloorToInt(g.transform.position))
                {
                    Destroy(tempGloob.gameObject);
                    return;
                }
                    
            }
            if (w > 8 && w != 16)
            {
                foreach(Gloob g in gloobList)
                {
                    if (g.blue)
                    {
                        g.gameObject.transform.position -= new Vector3(.1f, 0);
                    }
                }
                Destroy(tempGloob.gameObject);
                return;
            }
            List<Gloob> gloobsToDestroy = new List<Gloob>();
            foreach (Gloob g in gloobList)
            {
                gloobsToDestroy.Add(g);
            }
            foreach (Gloob g in gloobsToDestroy)
            {
                gloobList.Remove(g);
                Destroy(g.GetComponent<Collider2D>());
                g.animator.SetBool("explode", true);
            }
            
            if(w == 16)
            {
                //win
                Instantiate(superGloob, (Vector2)pos, Quaternion.identity);
                w -= 16;
                levelEnd();
                
            }
            if (w >= 8)
            {
                Vector2 mergedPos = tempGloob.CheckSurroundings(pos);

                Gloob mergedGloob = Instantiate(tempGloob, mergedPos, Quaternion.identity);
                mergedGloob.weight = 8;
                mergedGloob.transform.localScale = Vector2.one;
                w -= 8;
            }
            if (w >= 4)
            {
                Vector2 mergedPos = tempGloob.CheckSurroundings(pos);

                Gloob mergedGloob = Instantiate(tempGloob, mergedPos, Quaternion.identity);
                mergedGloob.weight = 4;
                mergedGloob.transform.localScale = Vector2.one/1.5f;
                w -= 4;
            }
            if (w >= 2)
            {
                Vector2 mergedPos = tempGloob.CheckSurroundings(pos);

                Gloob mergedGloob = Instantiate(tempGloob, mergedPos, Quaternion.identity);
                mergedGloob.weight = 2;
                mergedGloob.transform.localScale = Vector2.one / (1.5f* 1.5f);
                w -= 4;
            }
            if(w >= 1)
            {
                Vector2 mergedPos = tempGloob.CheckSurroundings(pos);

                Gloob mergedGloob = Instantiate(tempGloob, mergedPos, Quaternion.identity);
                mergedGloob.weight = 2;
                mergedGloob.transform.localScale = Vector2.one / (1.5f * 1.5f * 1.5f);
                w -= 1;
            }
            Destroy(tempGloob.gameObject);
        }
        
    }

    void OnSplit()
    {
        
        List<Gloob> gloobsToDestroy = new List<Gloob>();
        foreach(Gloob g in gloobList)
        {
            Gloob[] ngloobs = g.Split();
            if(ngloobs != null)
            {
                gloobsToDestroy.Add(g);
            }
        }
        foreach(Gloob g in gloobsToDestroy)
        {
            gloobList.Remove(g);

            Destroy(g.GetComponent<Collider2D>());
            g.animator.SetBool("explode", true);
        }
    }

    public void RemoveGloob(GameObject g)
    {
        gloobList.Remove(g.GetComponent<Gloob>());
    }
}
