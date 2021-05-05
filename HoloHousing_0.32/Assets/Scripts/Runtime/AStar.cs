using System.Collections;
using System.Collections.Generic;
// using System.Linq;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using Housing.Component;

using UnityEngine;

namespace Housing
{
    [DisableAutoCreation]
    public class AStar
    {
        // static public List<Squarea> SquareaList = new List<Squarea>();
        List<Squarea> noneList = new List<Squarea>();
        //未探索Squarea
        List<Node> openList = new List<Node>();
        //探索可能Node
        List<Node> closeList = new List<Node>();
        //探索済みNode
        int2 start;
        int2 goal;
        
        public AStar (int2 _start, int2 _goal, List<Squarea> _squareas)
        {
            start = _start;
            goal = _goal;
            
            // // noneList = new List<Squarea>(SquareaList);
            // foreach(Squarea s in SquareaList)
            // {
            //     noneList.Add(s);
            // }
            noneList = _squareas;
            Squarea startSquarea = new Squarea();
            foreach(Squarea s in noneList)
            {
                if(s.pos.x == start.x && s.pos.y == start.y)
                {
                    startSquarea = s;
                }
            }
            // noneList
            //     .Where(s => s.pos.x == start.x && s.pos.y == start.y)
            //     .First();
            Node StartNode = new Node(startSquarea, goal);
            openList.Add(StartNode);
            noneList.Remove(startSquarea);
            while(openList.Count > 0)
            {
                // openList.Sort((a,b)=>(a.totalCost - b.totalCost));
                // Node first = openList[0];

                // openList.OrderBy(s => s.totalCost);
                // Node first = openList.First();
                Node first = openList[0];
                for(int i=1;i<openList.Count;i++)
                {
                    if(openList[i].totalCost < first.totalCost)
                    {
                        first = openList[i];
                    }
                }
                Open(first);
            }
        }
        public List<int2> GetPath()
        {
            Node goalNode = new NullNode();
            foreach(Node n in closeList)
            {
                if(n.position.x==goal.x && n.position.y==goal.y)
                {
                    if(!n.isLock)
                    {
                        goalNode = n;
                    }
                }
            }
            
            //  = closeList
            //     .Where(n => n.position.x==goal.x && n.position.y==goal.y)
            //     .First();
            List<int2> path = new List<int2>();
            goalNode.GetPath(path);
            path.Reverse();
            if(path.Count>0)
            {
                path.RemoveAt(0);
            }
            return path;
        }
        
        private void Open(Node node)
        {
            int2 pos = node.position;
            List<Squarea> searchArea = new List<Squarea>();
            foreach(Squarea s in noneList)
            {
                if( (pos.x-1 <= s.pos.x && s.pos.x <= pos.x+1 && s.pos.y == pos.y) ||
                    (pos.y-1 <= s.pos.y && s.pos.y <= pos.y+1 && s.pos.x == pos.x))
                {
                    searchArea.Add(s);
                }
            }
            // noneList.Where(s => 
            // (
            //     (pos.x-1 <= s.pos.x && s.pos.x <= pos.x+1 && s.pos.y == pos.y) ||
            //     (pos.y-1 <= s.pos.y && s.pos.y <= pos.y+1 && s.pos.x == pos.x)
            // )));
            foreach(Squarea s in searchArea)
            {
                Node newNode = new Node(s, goal, node);
                if(newNode.isLock)
                {
                    closeList.Add(newNode);
                }
                else if(!newNode.isLock)
                {
                    openList.Add(newNode);
                }
                noneList.Remove(s);
            }
            Close(node);
        }
        private void Close(Node node)
        {
            openList.Remove(node);
            closeList.Add(node);
        }
    }
    
    public class Node
    {
        public Node parent;
        public int2 position;
        public int realCost;
        public int maybeCost;
        public int totalCost;
        public bool isLock;

        public Node(Squarea squarea, int2 goal){
            parent = new NullNode();
            position = squarea.pos;
            realCost = 0;
            maybeCost = math.abs(position.x - goal.x) + math.abs(position.y - goal.y);
            totalCost = realCost + maybeCost;
            isLock = squarea.isLock;
        }
        public Node(Squarea squarea, int2 goal, Node parentNode)
        {
            parent = parentNode;
            position = squarea.pos;
            realCost = parent.realCost + 1;
            maybeCost = math.abs(position.x - goal.x) + math.abs(position.y - goal.y);
            totalCost = realCost + maybeCost;
            isLock = squarea.isLock;
        }
        public Node() {}
        public virtual void GetPath(List<int2> path)
        {
            path.Add(position);
            if(parent != null)
            {
                if(parent.parent != null)
                {
                    parent.GetPath(path);
                }
            }
        }
    }
    public class NullNode : Node
    {
        public override void GetPath(List<int2> path) {Debug.Log("hoge");}
    }
}
