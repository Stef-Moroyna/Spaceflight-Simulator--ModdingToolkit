using System;
using System.Collections.Generic;
using System.Linq;

namespace SFS.World
{
    public interface I_JointNode
    {
        bool IsSoft { get; }
        bool ShouldDetach(I_JointNode node);
    }

    [Serializable]
    public class JointGroup<T> where T : class, I_JointNode
    {
        public List<Joint<T>> joints = new List<Joint<T>>();
        public List<T> objects = new List<T>();
        public T root;


        public JointGroup(T root)
        {
            this.root = root;


        }
        public JointGroup(List<Joint<T>> joints, List<T> objects, T root)
        {
            this.joints = joints;
            this.objects = objects;
            this.root = root;
        }




        // Creates a joint and adds it to the group
        public Joint<T> CreateJoint(Joint<T> joint)
        {
            // Checks if already has joint
            if (joints.Contains(joint))
                return joint;

            joints.Add(joint);

            // Try to add joint objects
            if (!objects.Contains(joint.a))
                objects.Add(joint.a);
            if (!objects.Contains(joint.b))
                objects.Add(joint.b);

            return joint;
        }

        public bool Contains(Joint<T> joint) => joints.Contains(joint);
        public bool Contains(T obj) => objects.Contains(obj);

        public void DestroyJoints(T obj, out List<JointGroup<T>> newGroups)
        {
            newGroups = new List<JointGroup<T>>();

            foreach (Joint<T> joint in GetConnectedJoints(obj))
            {
                // Assume joint is in this group
                JointGroup<T> rocketJoints = this;

                // If joint is not in group find its group
                if (!joints.Contains(joint))
                    foreach (JointGroup<T> group in newGroups)
                        if (group.joints.Contains(joint))
                        {
                            rocketJoints = group;
                            break;
                        }

                rocketJoints.DestroyJoint(joint, out bool split, out JointGroup<T> newGroup);

                if (split)
                    newGroups.Add(newGroup);
            }
        }
        public void DestroyJoint(Joint<T> joint, out bool split, out JointGroup<T> newGroup)
        {
            split = false;
            newGroup = null;

            // Remove joint
            joints.Remove(joint);

            // Check if joint created a split
            HashSet<Joint<T>> softJoints = new HashSet<Joint<T>>();
            if (ConnectAfterSplitRecursively(joint.a, joint.b, new HashSet<Joint<T>>(), ref softJoints))
                return;

            softJoints.RemoveWhere(x => !x.ShouldDetach);
            softJoints.ForEach(x => joints.Remove(x));

            // Moves disconnected parts to new group
            split = true;
            newGroup = new JointGroup<T>(joint.b);
            MoveObjectsAndJointsRecursively(joint.b, newGroup);

            // If root is now in the other group, switch this group with newGroup
            if (root != null && newGroup.Contains(root))
            {
                JointGroup<T> temp = newGroup;

                newGroup = new JointGroup<T>(joints, objects, objects[0]);

                joints = temp.joints;
                objects = temp.objects;
            }
        }

        public Joint<T>[] GetConnectedJoints(T obj)
        {
            List<Joint<T>> connectedJoints = new List<Joint<T>>();

            foreach (Joint<T> joint in joints)
                if (joint.IsConnectedTo(obj))
                    connectedJoints.Add(joint);

            return connectedJoints.ToArray();
        }

        // Returns a list of connections as a tree starting with provided root object
        public Branch[] GetObjectsAsTree(T rootObject, Func<T, bool> filter = null)
        {
            List<Branch> connections = new List<Branch>();
            PopulateConnectionTreeRecursively(new Branch(rootObject), connections, new HashSet<Joint<T>>(), filter);
            return connections.ToArray();
        }

        // Returns an array of groups of linked objects
        public List<T>[] GetLinkedObjectGroups(Func<T, bool> filter, Func<T, T, bool> comparer = null)
        {
            List<List<T>> groups = new List<List<T>>();
            GetLinkedObjectGroupsRecursively(root, filter == null || filter(root), ref groups, 0, filter, comparer, new HashSet<Joint<T>>(), new HashSet<T>());

            // Merge connected groups
            foreach (List<T> group in groups)
                MergeConnectedGroups(groups, group, comparer);
        
            // Remove now empty groups
            for (int i = groups.Count; i-- > 0;)
                if (groups[i].Count == 0)
                    groups.RemoveAt(i);

            return groups.ToArray();
        }

        void GetLinkedObjectGroupsRecursively(T obj, bool add, ref List<List<T>> groups, int groupIndex, Func<T, bool> filter, Func<T, T, bool> comparer, HashSet<Joint<T>> traversedJoints, HashSet<T> traversedObjects)
        {
            if (traversedObjects.Contains(obj))
                return;

            traversedObjects.Add(obj);

            // Evaluate whether or not to add object
            if (add)
            {
                if (groupIndex == groups.Count)
                {
                    List<T> group = new List<T>();
                    groups.Add(group);
                }

                groups[groupIndex].Add(obj);
            }

            // Move onto connections
            foreach (Joint<T> joint in GetConnectedJoints(obj))
            {
                if (traversedJoints.Contains(joint))
                    continue;

                traversedJoints.Add(joint);

                T other = joint.GetOtherObject(obj);

                bool addOther = filter == null || filter(other);

                int index = (add && addOther) ? comparer == null || comparer(obj, other) ? groupIndex : groups.Count : groups.Count;

                GetLinkedObjectGroupsRecursively(other, addOther, ref groups, index, filter, comparer, traversedJoints, traversedObjects);
            }
        }

        void MergeConnectedGroups(List<List<T>> groups, List<T> groupA, Func<T, T, bool> comparer = null)
        {
            List<T> groupsToMerge = new List<T>();

            foreach (T objA in groupA)
            {
                foreach (Joint<T> joint in GetConnectedJoints(objA))
                {
                    foreach (List<T> groupB in groups)
                    {
                        if (groupA == groupB)
                            continue;

                        foreach (T objB in groupB)
                        {
                            if (joint.IsConnectedTo(objB) && (comparer == null || comparer(objA, objB)))
                            {
                                groupsToMerge.AddRange(groupB);
                                groupB.Clear();
                                break;
                            }
                        }
                    }
                }
            }

            groupA.AddRange(groupsToMerge);
        }

        // Returns an array of lists of object groups
        public List<T>[] GetGroupedObjectsByFilter(Func<T, bool> filter, Func<T, T, bool> comparer = null)
        {
            List<List<T>> groups = new List<List<T>>();

            // Loop through objects
            foreach (T obj in objects)
            {
                // If object isn't accepted by filter skip
                if (filter != null && !filter(obj))
                    continue;

                // Assume no group will accept the object
                bool uncontained = true;

                // Loop through groups
                foreach (List<T> group in groups)
                {
                    // If object matches group or if there is no comparer (only one group outputs)
                    if (comparer == null || comparer(group[0], obj))
                    {
                        // Add object to group
                        group.Add(obj);
                        // Now we no object is contained
                        uncontained = false;
                    }
                }

                // If no group contained the object create a new group for it
                if (uncontained)
                {
                    List<T> group = new List<T>();
                    group.Add(obj);
                    groups.Add(group);
                }
            }

            return groups.ToArray();
        }

        // Merges multiple groups into one new group
        public static JointGroup<T> Merge(params JointGroup<T>[] groups)
        {
            JointGroup<T> mergedGroup = new JointGroup<T>(groups[0].objects[0]);

            foreach (JointGroup<T> group in groups)
            {
                foreach (Joint<T> joint in group.joints)
                    if (!mergedGroup.Contains(joint))
                        mergedGroup.joints.Add(joint);

                foreach (T obj in group.objects)
                    if (!mergedGroup.Contains(obj))
                        mergedGroup.objects.Add(obj);
            }

            return mergedGroup;
        }

        // Returns true if objects connected by removed joint still connect through this joint group
        bool ConnectAfterSplitRecursively(T a, T b, HashSet<Joint<T>> traversedJoints, ref HashSet<Joint<T>> softJoints)
        {
            // Loop through connected joints
            Joint<T>[] joints = GetConnectedJoints(a);
            foreach (Joint<T> joint in joints)
            {
                // Skip duplicate checks
                if (traversedJoints.Contains(joint))
                    continue;

                if (joint.IsSoft)
                    softJoints.Add(joint);

                traversedJoints.Add(joint);

                // Check for connections
                T other = joint.GetOtherObject(a);

                if (other == b)
                    return true;

                if (ConnectAfterSplitRecursively(other, b, traversedJoints, ref softJoints) && traversedJoints.All(x => x.IsHard))
                {
                    softJoints.Clear();
                    return true;
                }
            }

            return false;
        }

        // Recursively removes all joints in group
        void MoveObjectsAndJointsRecursively(T obj, JointGroup<T> newGroup)
        {
            if (objects.Contains(obj))
                objects.Remove(obj);

            if (!newGroup.objects.Contains(obj))
                newGroup.objects.Add(obj);

            // Loop through connected joints
            foreach (Joint<T> joint in GetConnectedJoints(obj))
            {
                if (!joints.Contains(joint))
                    continue;

                // Transfer joint from this group to new group and recursively remove joints from the other object connected to joint
                joints.Remove(joint);
                newGroup.CreateJoint(new Joint<T>(joint.a, joint.b, joint.anchor));
                MoveObjectsAndJointsRecursively(joint.GetOtherObject(obj), newGroup);
            }
        }

        void PopulateConnectionTreeRecursively(Branch connection, List<Branch> connections, HashSet<Joint<T>> traversedJoints, Func<T, bool> filter)
        {
            if (connections.Contains(connection) || filter != null && !filter(connection.data))
                return;

            connections.Add(connection);

            foreach (Joint<T> joint in GetConnectedJoints(connection.data))
            {
                if (traversedJoints.Contains(joint))
                    continue;

                traversedJoints.Add(joint);

                Branch child = new Branch(joint.GetOtherObject(connection.data));

                if (!connections.Contains(child))
                {
                    if (!connection.children.Contains(child))
                        connection.children.Add(child);
                
                    PopulateConnectionTreeRecursively(child, connections, traversedJoints, filter);
                }
            }
        }


        [Serializable]
        public class Branch : IEquatable<Branch>
        {
            public T data;
            public List<Branch> children = new List<Branch>();
            public Branch(T data) => this.data = data;
            bool IEquatable<Branch>.Equals(Branch other) => data == other?.data;
        }
    }
}