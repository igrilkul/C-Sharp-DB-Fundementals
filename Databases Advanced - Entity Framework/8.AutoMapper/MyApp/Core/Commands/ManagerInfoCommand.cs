﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Commands.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class ManagerInfoCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public ManagerInfoCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {

            var managerId = int.Parse(inputArgs[0]);

            var manager = this.context.Employees
                .Include(m=>m.ManagerEmployees)
                .FirstOrDefault(x=>x.Id==managerId);

            if(manager == null)
            {
                throw new ArgumentNullException("Manager not found.");
            }

            var managerDto = this.mapper.CreateMappedObject<ManagerDto>(manager);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{managerDto.FirstName} {managerDto.LastName} | Employees: {managerDto.ManagerEmployees.Count}");
            foreach(var emp in managerDto.ManagerEmployees)
            {
                sb.AppendLine($"   - {emp.FirstName} {emp.LastName} - ${emp.Salary:F2}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
