using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathfinderScript : MonoBehaviour {

    public Transform movementNodesObject;
    public Transform pointNodesObject;
    public Transform intersectionNodesObject;

    public Transform[] oneIntersectionPaths;
    public Transform[] mainPaths;
    public Transform[] pointNodes;

    public string GetPointLocationName(Transform pointNode) {

        return pointNode.GetComponent<PointNodeScript>().pointLocationName;

    }

    public Transform GetParentPath(Transform pointNode) {

        return pointNode.GetComponent<PointNodeScript>().parentPath;

    }

    private bool PathsConnect(Transform pathA, Transform pathB) {

        bool connects = false;

        foreach (Transform path in GetConnectingPaths(pathA)) {

            if (path == pathB) {

                connects = true;
                break;

            }

        }

        return connects;

    }

    private Transform[] GetPathNodes(Transform path) {

        return path.GetComponent<PathScript>().pathNodes;

    }

    private Transform[] GetConnectingPaths(Transform path) {

        return path.GetComponent<PathScript>().connectingPaths;

    }

    private Transform[] GetMainPathsFromPath(Transform path) {

        List<Transform> connectingMainPaths = new List<Transform>();
        
        foreach (Transform connPath in GetConnectingPaths(path)) {

            foreach (Transform mainPath in mainPaths) {

                if (connPath == mainPath) {

                    connectingMainPaths.Add(connPath);

                }

            }

        }

        return connectingMainPaths.ToArray();

    }

    private int GetMainPathId(Transform mainPath) {

        return mainPaths.ToList().IndexOf(mainPath);

    }

    private int GetOneIntersectPathId(Transform path) {

        return oneIntersectionPaths.ToList().IndexOf(path);

    }

    private Transform GetCommonMainPath(Transform pathA, Transform pathB) {

        Transform[] mainPathsFromPathA = GetMainPathsFromPath(pathA);
        Transform[] mainPathsFromPathB = GetMainPathsFromPath(pathB);

        foreach (Transform mainPathFromPathA in mainPathsFromPathA) {

            foreach (Transform mainPathFromPathB in mainPathsFromPathB) {

                if (mainPathFromPathA == mainPathFromPathB) {

                    return mainPathFromPathA;

                }

            }

        }

        return null;

    }

    private Transform[] GetCommonMainPathsAsNodes(Transform pathA, Transform pathB) {

        Transform[] mainPathsFromPathA = GetMainPathsFromPath(pathA);
        Transform[] mainPathsFromPathB = GetMainPathsFromPath(pathB);

        int indexOfPathA = GetOneIntersectPathId(pathA);
        int indexOfPathB = GetOneIntersectPathId(pathB);

        List<int> mainPathIds = new List<int>();

        if (indexOfPathA > indexOfPathB) {

            foreach (Transform mainPathFromPathB in mainPathsFromPathB) {

                mainPathIds.Add(GetMainPathId(mainPathFromPathB));

            }

        }
        else {

            foreach (Transform mainPathFromPathA in mainPathsFromPathA) {

                mainPathIds.Add(GetMainPathId(mainPathFromPathA));

            }

        }

        int minPathId = mainPathIds.Max();
        
        int index = minPathId;
        bool foundEndPath = false;

        List<Transform> nodesList = new List<Transform>();

        while (!foundEndPath) {

            Transform nextMainPath = mainPaths[index];

            if (indexOfPathA > indexOfPathB) {

                if (PathsConnect(nextMainPath, pathA)) {

                    foundEndPath = true;

                }

                nodesList.AddRange(GetPathNodes(nextMainPath));

            }
            else {

                if (PathsConnect(nextMainPath, pathB)) {

                    foundEndPath = true;

                }

                nodesList.AddRange(GetPathNodes(nextMainPath));

            }

            index++;

        }

        if (indexOfPathA > indexOfPathB) {

            nodesList.Reverse();

        }

        return nodesList.ToArray();

    }

    private Transform[] GetCommonMainPathAsNodes(Transform pathA, Transform pathB) {

        Transform[] mainPathsFromPathA = GetMainPathsFromPath(pathA);
        Transform[] mainPathsFromPathB = GetMainPathsFromPath(pathB);

        foreach (Transform mainPathFromPathA in mainPathsFromPathA) {

            foreach (Transform mainPathFromPathB in mainPathsFromPathB) {

                if (mainPathFromPathA == mainPathFromPathB) {

                    int indexOfPathA = GetOneIntersectPathId(pathA);
                    int indexOfPathB = GetOneIntersectPathId(pathB);

                    if (indexOfPathA > indexOfPathB) {

                        List<Transform> asList = GetPathNodes(mainPathFromPathA).ToList();
                        asList.Reverse();

                        return asList.ToArray();

                    }
                    else {

                        return GetPathNodes(mainPathFromPathA);

                    }

                }

            }

        }

        return null;

    }

    public Transform GetCommonNode(Transform pathA, Transform pathB) {

        if (PathsConnect(pathA, pathB)) {

            Transform[] pathANodes = GetPathNodes(pathA);
            Transform[] pathBNodes = GetPathNodes(pathB);

            Transform[] startAndEndNodesA = new Transform[] {pathANodes.First(), pathANodes.Last()};
            Transform[] startAndEndNodesB = new Transform[] {pathBNodes.First(), pathBNodes.Last()};

            foreach (Transform nodeA in startAndEndNodesA) {

                foreach (Transform nodeB in startAndEndNodesB) {

                    if (nodeA == nodeB) {

                        return nodeA;

                    }

                }

            }

        }

        return null;

    }

    private Transform[] GetPathNodesInRange(Transform path, Transform pointNodeA, Transform pointNodeB) {

        List<Transform> nodesInPath = GetPathNodes(path).ToList();

        int nodeAIndex = nodesInPath.IndexOf(pointNodeA);
        int nodeBIndex = nodesInPath.IndexOf(pointNodeB);

        if (nodeAIndex > nodeBIndex) {

            Transform[] allNodesBetweenInclusive = nodesInPath.Skip(nodeBIndex).Take(nodeAIndex - nodeBIndex + 1).Reverse().ToArray();
            return allNodesBetweenInclusive;

        }
        else {

            Transform[] allNodesBetweenInclusive = nodesInPath.Skip(nodeAIndex).Take(nodeBIndex - nodeAIndex + 1).ToArray();
            return allNodesBetweenInclusive;

        }

    }

    public Transform[] BuildPath(Transform pointNodeStart, Transform pointNodeEnd) {

        Transform pathA = GetParentPath(pointNodeStart);
        Transform pathB = GetParentPath(pointNodeEnd);

        if (pathA == pathB) {

            return GetPathNodesInRange(pathA, pointNodeStart, pointNodeEnd);

        }
        else if (PathsConnect(pathA, pathB)) {

            Transform sharedNode = GetCommonNode(pathA, pathB);

            if (sharedNode != null) {

                Transform[] section1 = GetPathNodesInRange(pathA, pointNodeStart, sharedNode);
                Transform[] section2 = GetPathNodesInRange(pathB, sharedNode, pointNodeEnd);

                List<Transform> pathList = new List<Transform>();
                pathList.AddRange(section1);
                pathList.AddRange(section2);
                return pathList.ToArray();

            }

        }
        else {

            Transform pathMiddle = GetCommonMainPath(pathA, pathB);
            
            if (pathMiddle != null) {

                Transform[] sectionMiddle = GetCommonMainPathAsNodes(pathA, pathB);

                Transform[] section1 = GetPathNodesInRange(pathA, pointNodeStart, GetCommonNode(pathA, pathMiddle));
                Transform[] section2 = GetPathNodesInRange(pathB, GetCommonNode(pathB, pathMiddle), pointNodeEnd);

                List<Transform> pathList = new List<Transform>();
                pathList.AddRange(section1);
                pathList.AddRange(sectionMiddle);
                pathList.AddRange(section2);
                return pathList.ToArray();

            }
            else {

                Transform[] sectionMiddle = GetCommonMainPathsAsNodes(pathA, pathB);

                Transform[] section1 = GetPathNodesInRange(pathA, pointNodeStart, GetCommonNode(pathA, pathMiddle));
                Transform[] section2 = GetPathNodesInRange(pathB, GetCommonNode(pathB, pathMiddle), pointNodeEnd);

                List<Transform> pathList = new List<Transform>();
                pathList.AddRange(section1);
                pathList.AddRange(sectionMiddle);
                pathList.AddRange(section2);

                if (!pointNodes.Contains(pathList[pathList.Count - 1])) {

                    pathList.RemoveAt(pathList.Count - 1);

                }

                if (!pointNodes.Contains(pathList[0])) {

                    pathList.RemoveAt(0);

                }

                return pathList.ToArray();

            }

        }

        Transform[] tempReturn = new Transform[] {pointNodeStart, pointNodeEnd};

        return tempReturn;

    }

    private void HideChildrenSprites(Transform parentObj) {

        for (int i = 0; i < parentObj.childCount; i++) {

            Transform child = parentObj.GetChild(i);
            SpriteRenderer childRenderer = child.GetComponent<SpriteRenderer>();

            childRenderer.enabled = false;

        }

    } 

    void Start() {

        HideChildrenSprites(pointNodesObject);
        HideChildrenSprites(movementNodesObject);
        HideChildrenSprites(intersectionNodesObject);

    }

}
