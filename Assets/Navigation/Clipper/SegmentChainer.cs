// PolygonClipper is a deterministic Version of PolyBoolCS is Copyright(c) 2018 Kevin Setiono
// The PolyBoolCS port is Copyright(c) 2017 StagPoint Software(@DoobiusGanes, web: stagpoint.com)
// polybooljs is (c) Copyright 2016, Sean Connelly (@voidqk), http://syntheti.cc
// MIT License

using System;
using System.Collections.Generic;

namespace Navigation.Clipper
{
    public class SegmentChainer
    {
        private List<PointList> chains;
        private List<PointList> regions;

        private SegmentChainerMatch first_match;
        private SegmentChainerMatch second_match;
        private SegmentChainerMatch next_match;

        public List<PointList> chain(SegmentList segments)
        {
            this.chains = new List<PointList>();
            this.regions = new List<PointList>();

            foreach (var seg in segments)
            {
                var pt1 = seg.start;
                var pt2 = seg.end;

                if (Epsilon.pointsSame(pt1, pt2))
                {
                    UnityEngine.Debug.Log("PolyBool: Warning: Zero-length segment detected; your epsilon is probably too small or too large");
                    continue;
                }

                first_match = new SegmentChainerMatch()
                {
                    index = 0,
                    matches_head = false,
                    matches_pt1 = false
                };

                second_match = new SegmentChainerMatch()
                {
                    index = 0,
                    matches_head = false,
                    matches_pt1 = false
                };

                next_match = first_match;

                for (var i = 0; i < chains.Count; i++)
                {
                    var chain = chains[i];
                    var head = chain[0];
                    var head2 = chain[1];
                    var tail = chain[chain.Count - 1];
                    var tail2 = chain[chain.Count - 2];

                    if (Epsilon.pointsSame(head, pt1))
                    {
                        if (setMatch(i, true, true))
                            break;
                    }
                    else if (Epsilon.pointsSame(head, pt2))
                    {
                        if (setMatch(i, true, false))
                            break;
                    }
                    else if (Epsilon.pointsSame(tail, pt1))
                    {
                        if (setMatch(i, false, true))
                            break;
                    }
                    else if (Epsilon.pointsSame(tail, pt2))
                    {
                        if (setMatch(i, false, false))
                            break;
                    }
                }

                if (next_match == first_match)
                {
                    // we didn't match anything, so create a new chain
                    chains.Add(new PointList() { pt1, pt2 });

                    continue;
                }

                if (next_match == second_match)
                {
                    // we matched a single chain

                    // add the other point to the apporpriate end, and check to see if we've closed the
                    // chain into a loop

                    var index = first_match.index;
                    var pt = first_match.matches_pt1 ? pt2 : pt1; // if we matched pt1, then we add pt2, etc
                    var addToHead = first_match.matches_head; // if we matched at head, then add to the head

                    var chain = chains[index];
                    var grow = addToHead ? chain[0] : chain[chain.Count - 1];
                    var grow2 = addToHead ? chain[1] : chain[chain.Count - 2];
                    var oppo = addToHead ? chain[chain.Count - 1] : chain[0];
                    var oppo2 = addToHead ? chain[chain.Count - 2] : chain[1];

                    if (Epsilon.pointsCollinear(grow2, grow, pt))
                    {
                        // grow isn't needed because it's directly between grow2 and pt:
                        // grow2 ---grow---> pt
                        if (addToHead)
                        {
                            chain.RemoveAt(0);
                        }
                        else
                        {
                            chain.RemoveAt(chain.Count - 1);
                        }
                        grow = grow2; // old grow is gone... new grow is what grow2 was
                    }

                    if (Epsilon.pointsSame(oppo, pt))
                    {
                        // we're closing the loop, so remove chain from chains
                        chains.RemoveAt(index);

                        if (Epsilon.pointsCollinear(oppo2, oppo, grow))
                        {
                            // oppo isn't needed because it's directly between oppo2 and grow:
                            // oppo2 ---oppo--->grow
                            if (addToHead)
                            {
                                chain.RemoveAt(chain.Count - 1);
                            }
                            else
                            {
                                chain.RemoveAt(0);
                            }
                        }

                        // we have a closed chain!
                        regions.Add(chain);
                        continue;
                    }

                    // not closing a loop, so just add it to the apporpriate side
                    if (addToHead)
                    {
                        chain.Insert(0, pt);
                    }
                    else
                    {
                        chain.Add(pt);
                    }

                    continue;
                }

                // otherwise, we matched two chains, so we need to combine those chains together

                var F = first_match.index;
                var S = second_match.index;

                var reverseF = chains[F].Count < chains[S].Count; // reverse the shorter chain, if needed
                if (first_match.matches_head)
                {
                    if (second_match.matches_head)
                    {
                        if (reverseF)
                        {
                            // <<<< F <<<< --- >>>> S >>>>
                            reverseChain(F);
                            // >>>> F >>>> --- >>>> S >>>>
                            appendChain(F, S);
                        }
                        else
                        {
                            // <<<< F <<<< --- >>>> S >>>>
                            reverseChain(S);
                            // <<<< F <<<< --- <<<< S <<<<   logically same as:
                            // >>>> S >>>> --- >>>> F >>>>
                            appendChain(S, F);
                        }
                    }
                    else
                    {
                        // <<<< F <<<< --- <<<< S <<<<   logically same as:
                        // >>>> S >>>> --- >>>> F >>>>
                        appendChain(S, F);
                    }
                }
                else
                {
                    if (second_match.matches_head)
                    {
                        // >>>> F >>>> --- >>>> S >>>>
                        appendChain(F, S);
                    }
                    else
                    {
                        if (reverseF)
                        {
                            // >>>> F >>>> --- <<<< S <<<<
                            reverseChain(F);
                            // <<<< F <<<< --- <<<< S <<<<   logically same as:
                            // >>>> S >>>> --- >>>> F >>>>
                            appendChain(S, F);
                        }
                        else
                        {
                            // >>>> F >>>> --- <<<< S <<<<
                            reverseChain(S);
                            // >>>> F >>>> --- >>>> S >>>>
                            appendChain(F, S);
                        }
                    }
                }
            }

            return regions;
        }

        private void reverseChain(int index)
        {
            chains[index].Reverse(); // gee, that's easy
        }

        private bool setMatch(int index, bool matches_head, bool matches_pt1)
        {
            // return true if we've matched twice
            next_match.index = index;
            next_match.matches_head = matches_head;
            next_match.matches_pt1 = matches_pt1;

            if (next_match == first_match)
            {
                next_match = second_match;
                return false;
            }

            next_match = null;

            return true; // we've matched twice, we're done here
        }

        private void appendChain(int index1, int index2)
        {
            // index1 gets index2 appended to it, and index2 is removed
            var chain1 = chains[index1];
            var chain2 = chains[index2];
            var tail = chain1[chain1.Count - 1];
            var tail2 = chain1[chain1.Count - 2];
            var head = chain2[0];
            var head2 = chain2[1];

            if (Epsilon.pointsCollinear(tail2, tail, head))
            {
                // tail isn't needed because it's directly between tail2 and head
                // tail2 ---tail---> head
                chain1.RemoveAt(chain1.Count - 1);
                tail = tail2; // old tail is gone... new tail is what tail2 was
            }

            if (Epsilon.pointsCollinear(tail, head, head2))
            {
                // head isn't needed because it's directly between tail and head2
                // tail ---head---> head2
                chain2.RemoveAt(0);
            }

            chain1.AddRange(chain2);
            chains.RemoveAt(index2);
        }
    }
}