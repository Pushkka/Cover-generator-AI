using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoverGenerator : MonoBehaviour
{
    public float CharterRadius;
    public float CharterHeight;

    public List<Line> AllLines = new List<Line>();
    public List<Line> AllSmallLines = new List<Line>();
    public GameObject CoverPrefab;
    GameObject _delta;

    [System.Serializable]
    public class Line
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 Dir;

        public Line(Vector3 a, Vector3 b, Vector3 dir)
        {
            A = a;
            B = b;
            Dir = dir;
        }
    }

    public void GenerateCovers()
    {
        AllLines.Clear();
        AllSmallLines.Clear();
        _delta = new GameObject("AllCovers");
        _delta.tag = "CoverPointParent";
        _delta.AddComponent<CoverLyst>();
        int[] Triangles = NavMesh.CalculateTriangulation().indices;
        Vector3[] Points = NavMesh.CalculateTriangulation().vertices;

        for (int i = 0; i < Triangles.Length; i += 3)
        {
            AllLines.Add(new Line(Points[Triangles[i]], Points[Triangles[i + 1]], new Vector3()));
            AllLines.Add(new Line(Points[Triangles[i]], Points[Triangles[i + 2]], new Vector3()));
            AllLines.Add(new Line(Points[Triangles[i + 1]], Points[Triangles[i + 2]], new Vector3()));
        }
        DeleteLines();
        DeleteLines();
        DeleteLines();

        CheckInBox();

        MergeLines();
        setDirection();

        deleteShit();
        CheckInBox();

        CheckSmallCover();

        CheckNormalCover();
    }






    void CheckInBox()
    {
        for (int i = 0; i < AllLines.Count; i++)
        {
            if (Physics.OverlapSphere((AllLines[i].A + Vector3.up), 0.1f).Length >= 1 || Vector3.Distance(AllLines[i].A, AllLines[i].B) < 0.5f || Physics.OverlapSphere((AllLines[i].A + Vector3.up), 0.3f).Length == 0 || Physics.OverlapSphere((AllLines[i].B + Vector3.up), 0.3f).Length == 0)
            {
                AllLines.RemoveAt(i);
                i -= 1;
            }
        }
    }

    void MergeLines()
    {
        for (int i = 0; i < AllLines.Count; i++)
        {
            for (int a = 0; a < AllLines.Count; a++)
            {
                //if (a == i)
                //    break;

                if (Vector3.Distance(AllLines[i].A, AllLines[a].A) < 0.5f && Vector3.Distance(AllLines[i].A, AllLines[a].A) > 0f)
                {
                    AllLines[a].A = AllLines[i].A - (AllLines[i].A - AllLines[a].A) * 0.5f;
                    AllLines[i].A = AllLines[a].A;
                    //break;
                }
                if (Vector3.Distance(AllLines[i].B, AllLines[a].A) < 0.5f && Vector3.Distance(AllLines[i].B, AllLines[a].A) > 0f)
                {
                    AllLines[a].A = AllLines[i].B - (AllLines[i].B - AllLines[a].A) * 0.5f;
                    AllLines[i].B = AllLines[a].A;
                    //break;
                }
                if (Vector3.Distance(AllLines[i].B, AllLines[a].B) < 0.5f && Vector3.Distance(AllLines[i].B, AllLines[a].B) > 0f)
                {
                    AllLines[a].B = AllLines[i].B - (AllLines[i].B - AllLines[a].B) * 0.5f;
                    AllLines[i].B = AllLines[a].B;
                    //break;
                }


            }
        }
    }

    void setDirection()
    {
        for (int i = 0; i < AllLines.Count; i++)
        {
            Vector3 Normalize = AllLines[i].B - AllLines[i].A;
            Vector3 Midle = AllLines[i].B - AllLines[i].A;
            Midle *= 0.5f;
            Normalize.Normalize();
            Normalize = new Vector3(Normalize.z, Normalize.y, -Normalize.x);

            if (Physics.OverlapSphere(((AllLines[i].A + Midle + (Normalize * 0.7f)) + Vector3.up), 0.1f).Length >= 1)
            {
                AllLines[i].Dir = Normalize * 0.5f;
            }
            if (Physics.OverlapSphere(((AllLines[i].A + Midle + (-Normalize * 0.7f)) + Vector3.up), 0.1f).Length >= 1)
            {
                AllLines[i].Dir = -Normalize * 0.5f;
            }
            if (AllLines[i].Dir == new Vector3())
            {
                AllLines.RemoveAt(i);
                i--;
            }
        }
    }

    void CheckSmallCover()
    {
        for (int i = 0; i < AllLines.Count; i++)
        {
            if (Physics.OverlapSphere((AllLines[i].A + Vector3.up), CharterRadius).Length >= 1 && Physics.OverlapSphere((AllLines[i].A + Vector3.up * CharterHeight), CharterRadius).Length == 0)
            {
                AllSmallLines.Add(AllLines[i]);
                AllLines.RemoveAt(i);
                i -= 1;
                continue;
            }
            if (Physics.OverlapSphere((AllLines[i].B + Vector3.up), CharterRadius).Length >= 1 && Physics.OverlapSphere((AllLines[i].B + Vector3.up * CharterHeight), CharterRadius).Length == 0)
            {
                AllSmallLines.Add(AllLines[i]);
                AllLines.RemoveAt(i);
                i -= 1;
                continue;
            }
        }

        for (int c = 0; c < 10; c++)
        {
            for (int i = 0; i < AllLines.Count; i++)
            {
                Vector3 start = new Vector3();
                Vector3 finish = new Vector3();
                Vector3 startSmall = new Vector3();
                start = AllLines[i].A;
                finish = AllLines[i].B;

                Vector3 Normalize = finish - start;
                Normalize.Normalize();
                Normalize *= 0.7f;
                start += Normalize;

                for (int a = 0; a < 200; a++)
                {
                    if (Vector3.Distance(start, finish) > 0.7f)
                    {
                        if (Physics.OverlapSphere((start + Vector3.up * CharterHeight + AllLines[i].Dir), CharterRadius).Length == 0)
                        {
                            AllLines[i].B = start;
                            startSmall = start;
                            break;
                        }
                        start += Normalize;
                    }
                    else
                        break;
                }
                for (int a = 0; a < 200; a++)
                {
                    if (Vector3.Distance(start, finish) > 0.7f)
                    {
                        if (Physics.OverlapSphere((start + Vector3.up * CharterHeight + AllLines[i].Dir), CharterRadius).Length != 0)
                        {
                            AllSmallLines.Add(new Line(startSmall, start, AllLines[i].Dir));
                            AllLines.Add(new Line(start, finish, AllLines[i].Dir));
                            break;
                        }
                        start += Normalize;
                    }
                    else
                    {
                        if (startSmall != new Vector3())
                        {
                            AllSmallLines.Add(new Line(startSmall, finish, AllLines[i].Dir));
                        }
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < AllSmallLines.Count; i++)
        {
            Vector3 start = new Vector3();
            Vector3 finish = new Vector3();
            if (Physics.OverlapSphere((AllSmallLines[i].A + Vector3.up * CharterHeight), CharterRadius).Length == 0)
            {
                start = AllSmallLines[i].A;
                finish = AllSmallLines[i].B;
            }
            else
            {
                start = AllSmallLines[i].B;
                finish = AllSmallLines[i].A;
            }
            Vector3 Normalize = finish - start;
            Normalize.Normalize();
            for (int a = 0; a < 100; a++)
            {
                if (Vector3.Distance(start, finish) > 1)
                {
                    if(Physics.OverlapSphere((start + Vector3.up * CharterHeight), CharterRadius).Length != 0)
                    {
                        AllLines.Add(new Line(finish, start, AllSmallLines[i].Dir));
                        if (Physics.OverlapSphere((AllSmallLines[i].A + Vector3.up * CharterHeight), CharterRadius).Length == 0)
                        {
                            AllSmallLines[i].B = start;
                        }
                        else
                        {
                            AllSmallLines[i].A = start;
                        }
                        break;
                    }
                    GameObject covr = Instantiate(CoverPrefab, start - AllSmallLines[i].Dir + Normalize * 0.5f, Quaternion.identity);
                    covr.GetComponent<Cover>().TypeOfCover = Cover.CoverType.Duck;
                    covr.transform.rotation = Quaternion.LookRotation(AllSmallLines[i].Dir);
                    covr.transform.parent = _delta.transform;
                    start += Normalize;
                }
                else
                    break;
            }
        }
    }

    void deleteShit()
    {
        int hight = 1;
        for (int y = 0; y < 10; y++)
        {
            for (int i = 0; i < AllLines.Count; i++)
            {
                for (int a = 0; a < AllLines.Count; a++)
                {
                    if (a == i)
                        continue;

                    Vector3 jopa = AllLines[i].Dir - AllLines[a].Dir;

                    if (jopa.magnitude < 0.2f)
                    {
                        if (Vector3.Distance(AllLines[i].A, AllLines[a].B) < 0.5f)
                        {
                            //Debug.Log("var4");
                            //Debug.DrawLine(AllLines[i].A + Vector3.up * hight, AllLines[i].B + Vector3.up * hight, Color.cyan, 10);
                            AllLines[i].A = AllLines[a].A;
                            //Debug.DrawLine(AllLines[a].A + Vector3.up * hight, AllLines[a].B + Vector3.up * hight, Color.red, 10);
                            AllLines.RemoveAt(a);
                            hight++;
                            break;

                        }
                        if (Vector3.Distance(AllLines[i].A, AllLines[a].A) < 0.5f)
                        {
                            //Debug.Log("var1");
                            //Debug.DrawLine(AllLines[i].A + Vector3.up * hight, AllLines[i].B + Vector3.up * hight, Color.cyan, 10);
                            AllLines[i].A = AllLines[a].B;
                            AllLines.RemoveAt(a);
                            hight++;
                            break;

                        }
                        if (Vector3.Distance(AllLines[i].B, AllLines[a].A) < 0.5f)
                        {
                            //Debug.Log("var2");
                            //Debug.DrawLine(AllLines[i].A + Vector3.up * hight, AllLines[i].B + Vector3.up * hight, Color.cyan, 10);
                            AllLines[i].B = AllLines[a].B;
                            //Debug.DrawLine(AllLines[a].A + Vector3.up * hight, AllLines[a].B + Vector3.up * hight, Color.red, 10);
                            AllLines.RemoveAt(a);
                            hight++;
                            break;

                        }
                        if (Vector3.Distance(AllLines[i].B, AllLines[a].B) < 0.5f)
                        {
                            //.Log("var3");
                            //Debug.DrawLine(AllLines[i].A + Vector3.up * hight, AllLines[i].B + Vector3.up * hight, Color.cyan, 10);
                            AllLines[i].B = AllLines[a].A;
                            AllLines.RemoveAt(a);
                            hight++;
                            break;

                        }
                    }
                }
            }
        }
    }


    void CheckNormalCover()
    {

        List<Vector3> points = new List<Vector3>();
        List<Vector3> Directions = new List<Vector3>();

        for (int i = 0; i < AllLines.Count; i++)
        {
            Vector3 Normalize = AllLines[i].B - AllLines[i].A;
            Normalize.Normalize();
            Normalize *= 0.5f;
            
            points.Add(AllLines[i].A - AllLines[i].Dir + Normalize);
            Directions.Add(AllLines[i].Dir);

            points.Add(AllLines[i].B - AllLines[i].Dir - Normalize);
            Directions.Add(AllLines[i].Dir);
        }

        for (int i = 0; i < points.Count; i++)
        {

            for (int a = 0; a < points.Count; a++)
            {
                if (i == a)
                    break;
                if (Vector3.Distance(points[i], points[a]) < 0.7f)
                {
                    points.RemoveAt(i);
                    points.RemoveAt(a);
                    Directions.RemoveAt(i);
                    Directions.RemoveAt(a);

                    i--;
                    break;
                }
            }
        }

        for (int i = 0; i < points.Count; i++)
        {
            GameObject covr = Instantiate(CoverPrefab, points[i], Quaternion.identity);
            covr.transform.parent = _delta.transform;
            covr.transform.rotation = Quaternion.LookRotation(Directions[i]);

            if (Physics.OverlapSphere((covr.transform.position + Vector3.up * CharterHeight + covr.transform.right + covr.transform.forward), CharterRadius).Length == 0)
            {
                covr.GetComponent<Cover>().Right = true;
            }
            else
            {
                if (Physics.OverlapSphere((covr.transform.position + Vector3.up * CharterHeight - covr.transform.right + covr.transform.forward), CharterRadius).Length == 0)
                {
                    covr.GetComponent<Cover>().Right = false;
                }
                else
                    DestroyImmediate(covr);
            }
        }
    }

    void DeleteLines()
    {
        for (int i = 0; i < AllLines.Count; i++)
        {
            bool deleted = false;

            for (int a = 0; a < AllLines.Count; a++)
            {
                if (a == i)
                    break;

                //я на полном серьезе считал что 10 + -3 != -3 + 10
                //я доблоеб, и забыл включить запись

                if (AllLines[i].A.magnitude + AllLines[i].B.magnitude == AllLines[a].A.magnitude + AllLines[a].B.magnitude)
                {
                    AllLines.RemoveAt(i);
                    AllLines.RemoveAt(a);
                    deleted = true;
                    break;
                }
            }
            if (deleted)
            {
                i -= 1;
            }
        }
    }
    private void OnDrawGizmos()
    {
        foreach (var item in AllLines)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(item.A,item.B);

            Vector3 Midle = item.B - item.A;
            Midle *= 0.5f;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(item.A + Midle, item.A + Midle + item.Dir);
        }
        foreach (var item in AllSmallLines)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(item.A,item.B);

            Vector3 Midle = item.B - item.A;
            Midle *= 0.5f;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(item.A + Midle, item.A + Midle + item.Dir);
        }
    }
}
