using Aoc.Common;
using System.Data;
using System.Linq;

namespace Aoc.Y2023.Day_19
{
    // https://adventofcode.com/2023/day/19
    public class Day_19 : ISolver
    {
        private class AttributeRange
        {
            public int LowerBound { get; set; }
            public int UpperBound { get; set; }

            public int Length => UpperBound - LowerBound;

            public AttributeRange(int lowerBound, int upperBound)
            {
                LowerBound = lowerBound;
                UpperBound = upperBound;
            }

            public AttributeRange GetPassingRange(int num, bool greaterThan)
            {
                if (greaterThan)
                {
                    if (num >= UpperBound) return new AttributeRange(0, 0);

                    if (num < LowerBound) return new AttributeRange(LowerBound, UpperBound);

                    return new AttributeRange(num + 1, UpperBound);
                }
                else
                {
                    if (num >= UpperBound) return new AttributeRange(LowerBound, UpperBound);

                    if (num <= LowerBound) return new AttributeRange(0, 0);

                    return new AttributeRange(LowerBound, num - 1);
                }
            }

            public AttributeRange GetFailingRange(int num, bool greaterThan)
            {
                if (greaterThan)
                {
                    if (num >= UpperBound) return new AttributeRange(LowerBound, UpperBound);

                    if (num <= LowerBound) return new AttributeRange(0, 0);

                    return new AttributeRange(LowerBound, num);
                }
                else
                {
                    if (num >= UpperBound) return new AttributeRange(0, 0);

                    if (num < LowerBound) return new AttributeRange(LowerBound, UpperBound);

                    return new AttributeRange(num, UpperBound);
                }
            }
        }

        private class Part
        {
            public Dictionary<string, int> Attributes { get; set; } = new Dictionary<string, int>();

            public int GetRating()
            {
                return Attributes.Sum(a => a.Value);
            }
        }

        private class Workflow
        {
            public string Name { get; set; }
            public List<Rule> Rules { get; set; }

            public Workflow(string name, List<Rule> rules)
            {
                Name = name;
                Rules = rules;
            }
        }

        private class Rule
        {
            public string? PartAttribute { get; set; }
            public string SendTo { get; set; } = string.Empty;
            public bool GreaterThan { get; set; }
            public bool LessThan { get; set; }
            public bool Unconditional { get; set; }
            public int Threshold { get; set; }

            public Rule(string? partAttribute, string sendTo, bool greaterThan, bool lessThan, bool unconditional, int threshold)
            {
                PartAttribute = partAttribute;
                SendTo = sendTo;
                GreaterThan = greaterThan;
                LessThan = lessThan;
                Unconditional = unconditional;
                Threshold = threshold;
            }

            public bool EvaluatePart(Part part, out string destination)
            {
                destination = SendTo;

                if (this.Unconditional)
                {
                    return true;
                }
                else if (GreaterThan)
                {
                    return part.Attributes[this.PartAttribute!] > this.Threshold;
                }
                else if (LessThan)
                {
                    return part.Attributes[this.PartAttribute!] < this.Threshold;
                }
                else
                {
                    throw new Exception($"Could not process rule for part");
                }
            }
        }

        public object Part1(IList<string> lines)
        {
            var (workflows, parts) = ParseLines(lines);

            var accepted = new List<Part>();
            var rejected = new List<Part>();

            var workflowsDictionary = workflows.ToDictionary(w => w.Name, w => w);

            foreach (var part in parts)
            {
                var destination = "in";

                while (destination != "A" && destination != "R")
                {
                    var workflow = workflowsDictionary[destination];
                    
                    var workflowRuleSucceded = false;
                    var workflowRuleIndex = 0;

                    while (!workflowRuleSucceded)
                    {
                        var rule = workflow.Rules[workflowRuleIndex];

                        var success = rule.EvaluatePart(part, out var nextDestination);

                        if (success)
                        {
                            workflowRuleSucceded = true;
                            destination = nextDestination;
                        }
                        else
                        {
                            workflowRuleIndex++;
                        }
                    }
                }

                if (destination == "A")
                {
                    accepted.Add(part);
                }
                else if (destination == "R")
                {
                    rejected.Add(part);
                }
                else
                {
                    throw new Exception($"Unexpected destination {destination}");
                }
            }

            var ratings = accepted.Select(p => p.GetRating());
            var sum = ratings.Sum();

            return sum;
        }

        public object Part2(IList<string> lines)
        {
            var (workflows, parts) = ParseLines(lines);
            var workflowsDictionary = workflows.ToDictionary(w => w.Name, w => w);

            var startingWorkflow = workflowsDictionary["in"];

            var startingRanges = new string[] { "x", "m", "a", "s" }.ToDictionary(c => c, _ => new AttributeRange(1, 4001));

            var acceptanceRanges = GetAcceptanceRanges(startingWorkflow, workflowsDictionary, ruleIndex: 0, startingRanges);

            var possibilites = acceptanceRanges.Select(ar => 1L * ar["x"].Length * ar["m"].Length * ar["a"].Length * ar["s"].Length);

            var sum = possibilites.Sum();

            return sum;
        }

        private static List<Dictionary<string, AttributeRange>> GetAcceptanceRanges(
            Workflow workflow, Dictionary<string, Workflow> workflows, int ruleIndex, Dictionary<string, AttributeRange> currentRanges)
        {
            var rule = workflow.Rules[ruleIndex];

            var ranges = new List<Dictionary<string, AttributeRange>>();

            if (rule.Unconditional && rule.SendTo == "R")
            {
                // No-op
                //ranges.Add(new string[] { "x", "m", "a", "s" }.ToDictionary(c => c, _ => new AttributeRange(0, 0)));
            }
            else if (rule.Unconditional && rule.SendTo == "A")
            {
                ranges.Add(currentRanges);
            }
            else if (rule.Unconditional)
            {
                var nextDestination = rule.SendTo;
                var nextWorkflow = workflows[nextDestination];

                ranges.AddRange(GetAcceptanceRanges(nextWorkflow, workflows, 0, currentRanges));
            }
            else
            {
                var partAttribute = rule.PartAttribute!;

                var passingRange = currentRanges[partAttribute].GetPassingRange(rule.Threshold, rule.GreaterThan);
                var failingRange = currentRanges[partAttribute].GetFailingRange(rule.Threshold, rule.GreaterThan);

                var nextDestination = rule.SendTo;

                if (nextDestination == "A")
                {
                    currentRanges[partAttribute] = passingRange;
                    ranges.Add(currentRanges);

                    var failingCopy = currentRanges.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    failingCopy[partAttribute] = failingRange;

                    ranges.AddRange(GetAcceptanceRanges(workflow, workflows, ruleIndex + 1, failingCopy));
                }
                else if (nextDestination == "R")
                {
                    // no-op for passing.

                    var failingCopy = currentRanges.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    failingCopy[partAttribute] = failingRange;

                    ranges.AddRange(GetAcceptanceRanges(workflow, workflows, ruleIndex + 1, failingCopy));
                }
                else
                {
                    var passingCopy = currentRanges.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    passingCopy[partAttribute] = passingRange;

                    var nextWorkflow = workflows[nextDestination];

                    ranges.AddRange(GetAcceptanceRanges(nextWorkflow, workflows, 0, passingCopy));

                    var failingCopy = currentRanges.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    failingCopy[partAttribute] = failingRange;

                    ranges.AddRange(GetAcceptanceRanges(workflow, workflows, ruleIndex + 1, failingCopy));
                }
            }

            return ranges;
        }

        private static (IEnumerable<Workflow> Workflows, IEnumerable<Part> Parts) ParseLines(IList<string> lines)
        {
            var workflowLines = lines.TakeWhile(l => !string.IsNullOrEmpty(l)).ToList();
            var partLines = lines.Skip(workflowLines.Count + 1);

            var workflows = new List<Workflow>();

            foreach (var workflowLine in workflowLines)
            {
                var name = workflowLine.Split("{").First();

                var rulesString = workflowLine.Split('{', '}')[1];

                var rulesStrings = rulesString.Split(',');

                var rules = new List<Rule>();

                foreach (var ruleString in rulesStrings)
                {
                    if (!ruleString.Contains(':'))
                    {
                        var rule = new Rule(partAttribute: null, sendTo: ruleString, greaterThan: false, lessThan: false, unconditional: true, threshold: 0);

                        rules.Add(rule);
                        continue;
                    }
                    else
                    {
                        var comparisonString = ruleString.Split(':');
                        var equation = comparisonString[0];
                        var destination = comparisonString[1];

                        if (equation.Contains('>'))
                        {
                            var components = equation.Split('>');
                            var attribute = components[0];
                            var threshold = components[1];

                            var rule = new Rule(attribute, destination, greaterThan: true, lessThan: false, unconditional: false, int.Parse(threshold));

                            rules.Add(rule);
                            continue;
                        }
                        else if (equation.Contains('<'))
                        {
                            var components = equation.Split('<');
                            var attribute = components[0];
                            var threshold = components[1];

                            var rule = new Rule(attribute, destination, greaterThan: false, lessThan: true, unconditional: false, int.Parse(threshold));

                            rules.Add(rule);
                            continue;
                        }
                        else
                        {
                            throw new Exception($"Could not parse rule {ruleString} in workflow {workflowLine}");
                        }
                    }
                }

                var workflow = new Workflow(name, rules);

                workflows.Add(workflow);
            }

            var parts = new List<Part>();

            foreach (var partLine in partLines)
            {
                var attributesString = partLine.Split('{', '}')[1];

                var attributes = new Dictionary<string, int>();

                var attributeStrings = attributesString.Split(',');

                foreach (var attributeString in attributeStrings)
                {
                    var statString = attributeString.Split('=');

                    var attribute = statString[0];
                    var value = statString[1];

                    attributes.Add(attribute, int.Parse(value));
                }

                var part = new Part {  Attributes = attributes };

                parts.Add(part);
            }

            return (workflows, parts);
        }
    }
}
