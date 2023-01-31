import React, { useState } from "react";
import { Project } from "../dataEntities/Project";
import { timeInMinutesToFormattedString } from "../utils/helpers";
import { SortingEnum } from "../dataEntities/SortingEnum";
interface ProjectTableProps {
  projects: Array<Project>;
}

export default function ProjectTable({ projects }: ProjectTableProps) {
  const [sortingByDeadline, setSortingByDeadline] = useState(SortingEnum.NONE);
  function handleSortingByDeadlineClick(): void {
    if (sortingByDeadline == 2) {
      setSortingByDeadline(0);
    } else {
      setSortingByDeadline(sortingByDeadline + 1);
    }
  }

  function addSortingIcon(): string {
    switch (sortingByDeadline) {
      case SortingEnum.NONE:
        return "↨";
      case SortingEnum.INCREMENTAL:
        return "↓";
      case SortingEnum.DECREMENTAL:
        return "↑";

      default:
        return "↨";
    }
  }

  function sortingFunction(project1: Project, project2: Project): number {
    switch (sortingByDeadline) {
      case SortingEnum.NONE:
        return project1.id - project2.id;
      case SortingEnum.INCREMENTAL:
        return project1.deadline.getTime() - project2.deadline.getTime();
      case SortingEnum.DECREMENTAL:
        return project2.deadline.getTime() - project1.deadline.getTime();

      default:
        return 0;
    }
  }
  return (
    <table className="table-fixed w-full">
      <caption>Projects summary</caption>
      <thead className="bg-gray-200">
        <tr>
          <th className="border px-4 py-2 w-12">#</th>
          <th className="border px-4 py-2">Project Name</th>
          <th className="border px-4 py-2">Customer</th>
          <th className="border px-4 py-2">Time spent </th>
          <th className="border px-4 py-2" onClick={handleSortingByDeadlineClick}>
            Deadline {addSortingIcon()}
          </th>
        </tr>
      </thead>
      <tbody>
        {projects.sort(sortingFunction).map((project) => {
          return (
            <tr key={project.id}>
              <td className="border px-4 py-2 w-12"> {project.id}</td>
              <td className="border px-4 py-2">{project.name}</td>
              <td className="border px-4 py-2">{project.customer}</td>
              <td className="border px-4 py-2">{timeInMinutesToFormattedString(project.totalTimeSpentInMinutes)}</td>
              <td className="border px-4 py-2">{project.deadline.toLocaleDateString()}</td>
            </tr>
          );
        })}
      </tbody>
    </table>
  );
}
