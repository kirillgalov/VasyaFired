using System;
using System.Collections.Generic;
using System.Linq;
using VasyaFiredLib.Organization;

namespace VasyaFiredLib.Dismissals
{
    public class DismissalService
    {
        public GetStampsResult GetStamps(Vasya vasya, DepartmentId q, Organization.Organization organization)
        {
            Stack<StampsCollection> result = new();
            ICycleChecker cycleChecker = new CycleChecker();
            
            bool noVisit = true, infinityCycle;
            DepartmentId next = vasya.A, current;
            
            result.Push(new StampsCollection());
            
            do
            {
                current = next;
                ref readonly Department department = ref organization.GetDepartment(current);
                StampsCollection currentStamps = result.Peek();
                next = ProcessDepartment(department, organization, currentStamps);
                cycleChecker.AddToVisited(current);
                infinityCycle = cycleChecker.IsInInfinityCycle(currentStamps, next, current);
                if (infinityCycle)
                    break;
                if (q == current)
                {
                    noVisit = false;
                    result.Push(new StampsCollection(result.Peek()));
                }
                
            } while (current != vasya.Z);

            result.Pop(); // выкидываем последний набор т.к. он полностью копирует предпоследний
            return new GetStampsResult
            {
                InfinityCycle = infinityCycle,
                NoVisit = noVisit,
                VisitCount = result.Count,
                StampsSets = result.Reverse().ToHashSet()
            };
        }

        private DepartmentId ProcessDepartment(Department department,
            Organization.Organization organization,
            StampsCollection currentStamps)
        {
            return department.RuleType switch
            {
                RuleType.Conditional => ProcessConditional(department.RuleId, organization, currentStamps),
                RuleType.Unconditional => ProcessUnconditional(department.RuleId, organization, currentStamps),
                _ => throw new Exception($"Неизвестный тип правила для отдела {department.RuleType}")
            };
        }
        
        private DepartmentId ProcessConditional(RuleId ruleId, 
            Organization.Organization organization,
            StampsCollection currentStamps)
        {
            ref readonly ConditionalRule rule = ref organization.GetConditionRule(ruleId);
            var (add, remove, goTo) = currentStamps.Contains(rule.S) 
                ? (rule.I, rule.J, rule.K) 
                : (rule.T, rule.R, rule.P);
            currentStamps.Add(add);
            currentStamps.Remove(remove);
            return goTo;
        }
        
        private DepartmentId ProcessUnconditional(RuleId ruleId, 
            Organization.Organization organization,
            StampsCollection currentStamps)
        {
            ref readonly UnconditionalRule rule = ref organization.GetUnconditionalRule(ruleId);
            currentStamps.Add(rule.I);
            currentStamps.Remove(rule.J);
            return rule.K;
        }
        
    }
}