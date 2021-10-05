using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using Managers.MapManager;
using UnityEngine;

// A*寻路
public class StarA
{
    public MapItemController[,] Controllers;
    public class Point : IComparable<Point>
    {
        public Vector3 Layout;
        public int x, y;
        public bool visible;
        public int f;
        public int g;
        public int h;
        public Point father;
        public Point child;
        public Point()
        {
            visible = true;
            f = 0;
            g = 0;
            h = 0;
        }
        public int CompareTo(Point p)
        {
            if (this.f > p.f)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }
    }
    List<Point> openList;
    public Point[,] points;
    int w, h;
    public void Init(int w, int h)
    {
        openList = new List<Point>();
        this.w = w;
        this.h = h;
        points = new Point[w, h];
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                points[i, j] = new Point();
                points[i, j].x = i;
                points[i, j].y = j;
            }
        }
    }

    private Point getMinPoint()
    {
        if (openList.Count == 0)
            return null;
        Point p = openList[0];
        for (int i = 1; i < openList.Count; i++)
        {
            if (p.f > openList[i].f)
            {
                p = openList[i];
            }
        }
        return p;
    }

    public void ReInitMap()
    {
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                points[i, j].child = null;
                points[i, j].father = null;
                points[i, j].visible = true;
                points[i, j].f = 0;
                points[i, j].g = 0;
                points[i, j].h = 0;
                Controllers[i, j].GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        openList = new List<Point>();
    }
    public void StartAs(Point start, Point end)
    {
        Point result = FindNewPoints(start, end);
        if (result == null)
        {
            if (openList.Count == 0)
            {
            }
            return;
        }
        StartAs(result, end);
    }

    private void GetH(Point p,Point end)
    {
        p.h = (Math.Abs(p.x - end.x) + Math.Abs(p.y - end.y)) * 10;
        p.f = p.g + p.h;
    }

    private Point FindNewPoints(Point start, Point end)
    {
        start.visible = false;
        openList.Remove(start);
        int x = start.x;
        int y = start.y;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i < 0 || i >= w || j < 0 || j >= h || (i == x && j == y))
                    continue;
                if (points[i,j].visible == false)
                    continue;
                int g = (Math.Abs(i - x) + Math.Abs(j - y)) == 2 ? start.g + 14 : start.g + 10;
                if (points[i, j].f != 0)
                {
                    if (points[i, j].g > g)
                    {
                        points[i, j].g = g;
                        GetH(points[i, j], end);
                        points[i, j].father = start;
                    }
                }
                else
                {
                    points[i, j].g = g;
                    GetH(points[i, j], end);
                    points[i, j].father = start;
                    openList.Add(points[i, j]);
                }
                if (i == end.x && j == end.y)
                {
                    return null;
                }
            }
        }
        return getMinPoint();
    }
  
    public void Link(Point end)
    {
        if (end == null)
            return;
        if (end.father != null)
        {
            end.father.child = end;
        }
        Controllers[end.x, end.y].GetComponent<SpriteRenderer>().color = Color.green;
        Link(end.father);
    }

}