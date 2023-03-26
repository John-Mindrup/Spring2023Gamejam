using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Gloob : MonoBehaviour
{
    private bool inMotion = false;
    private Rigidbody2D rb;
    private Vector2 Destination;
    private List<Pathnode> path;
    public bool blue;
    public int speed = 5;
    public int weight = 8;
    public bool hover = false;
    public bool fireProof = false;
    public int carriedWeight = 0;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void StopMoving()
    {
        if (inMotion)
        {
            animator.SetBool("moving", false);
            inMotion = false;
            StopAllCoroutines();
            Vector2Int newPos = Vector2Int.RoundToInt(transform.position);
            transform.position = new Vector2(newPos.x, newPos.y);
        }
    }

    public void doneSpawning()
    {
        animator.SetBool("Spawning", false);
    }

    public void deleteAfterExplode()
    {
        Destroy(this.gameObject);
    }

    public bool CanSplit()
    {
        if (!inMotion && weight > 1)
        {
            Vector2Int spaceA = Vector2Int.FloorToInt(this.transform.position);
            Vector2Int spaceB = CheckSurroundings(spaceA);
            Vector3 sa = new Vector3(spaceA.x, spaceA.y);
            Vector3 sb = new Vector3(spaceB.x, spaceB.y);
            if (spaceB.x != -1)
            {
                return true;
            }
        }
        return false;
    }

    public void carrying(bool c)
    {
        animator.SetBool("Carrying", c);
    }
    

    public Gloob[] Split()
    {
        if (!inMotion && weight > 1)
        {
            Vector2Int spaceA = Vector2Int.FloorToInt(this.transform.position);
            Vector2Int spaceB = CheckSurroundings(spaceA);
            Vector3 sa = new Vector3(spaceA.x, spaceA.y);
            Vector3 sb = new Vector3(spaceB.x, spaceB.y);
            if(spaceB.x != -1)
            {
                foreach (Transform child in this.gameObject.transform)
                {
                    child.gameObject.transform.parent = null;
                }
                this.hover = false;
                Gloob gloobA = Instantiate(this.gameObject, sa, Quaternion.identity).GetComponent<Gloob>();
                Gloob gloobB = Instantiate(this.gameObject, sb, Quaternion.identity).GetComponent<Gloob>();
                gloobA.transform.localScale = this.transform.localScale/1.5f;
                gloobB.transform.localScale = this.transform.localScale / 1.5f;
                gloobA.weight = weight/2;
                gloobB.weight = weight / 2;
                Gloob[] ret = new Gloob[2];
                ret[0] = gloobA;
                ret[1] = gloobB;
                return ret;
            }
        }
        return null;
    }

    private bool openSpace(Vector2Int space)
    {
        if (SceneStats.Instance.obstacles.GetTile<Tile>((Vector3Int)space) != null)
            return false;
        List<Collider2D> res = new List<Collider2D>();
        Physics2D.OverlapArea(space, (Vector2Int)(space + new Vector2Int(1,1)), new ContactFilter2D(), res);
        foreach(Collider2D c in res)
        {
            if (c.gameObject.GetComponent<Gloob>() != null)
                return false;
        }
        return true;
    }

    public Vector2Int CheckSurroundings(Vector2Int space)
    {
        //up
        if (space.y + 1 < AStar.Instance.height)
            if (openSpace(space + Vector2Int.up))
                return space + Vector2Int.up;
        //right
        if (space.x + 1 < AStar.Instance.width)
            if (openSpace(space + Vector2Int.right))
                return space + Vector2Int.right;
        //down
        if (space.y - 1 >= 0)
            if (openSpace(space + Vector2Int.down))
                return space + Vector2Int.down;
        //left
        if (space.x - 1 >= 0)
            if (openSpace(space + Vector2Int.left))
                return space + Vector2Int.left;
        return new Vector2Int(-1,-1);
    }

    private IEnumerator MoveTowardNode(List<Pathnode> list)
    {
        
        foreach(Pathnode node in list)
        {
            Vector3 nodepos = node.worldpos;
            while (this.transform.position != nodepos)
            {
                Vector3 dir = Vector3.MoveTowards(this.transform.position, nodepos, speed * Time.deltaTime) - this.transform.position;
                if(dir.x < 0)
                {
                    if(dir.y <= 0)
                    {
                        animator.SetInteger("direction", 3);
                    }
                    else
                    {
                        animator.SetInteger("direction", 2);
                    }
                }
                else
                {
                    if(dir.y <= 0)
                    {
                        animator.SetInteger("direction", 1);
                    }
                    else
                    {
                        animator.SetInteger("direction", 0);
                    }
                }
                this.transform.position = Vector3.MoveTowards(this.transform.position, nodepos, speed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            
        }
        inMotion = false;
        animator.SetBool("moving", false);
    }

    public void Move(Vector2 position)
    {
        if(inMotion) return;
        Destination = position;
        path = AStar.Instance.FindPath((int)this.transform.position.x, (int)this.transform.position.y, (int)Destination.x, (int)Destination.y, this);
        if (path != null)
        {
            inMotion = true;
            animator.SetBool("moving", true);
            StartCoroutine(MoveTowardNode(path));
        }
        else Debug.Log("No Path Detected");
        
    }
}
