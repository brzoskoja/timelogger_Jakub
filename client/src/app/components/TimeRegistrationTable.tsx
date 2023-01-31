import React, { useState } from "react";
import { TimeRegistration } from "../dataEntities/TimeRegistration";
import { timeInMinutesToFormattedString } from "../utils/helpers";
import { Project } from "../dataEntities/Project";

interface TimeRegistrationTableProps {
  timeRegistrations: Array<TimeRegistration>;
  projects: Array<Project>;
}

export default function TimeRegistrationTable({ timeRegistrations, projects }: TimeRegistrationTableProps) {
  const [selectedProjectId, setSelectedProjectId] = useState(0);

  function handleSelectProject(event: React.ChangeEvent<HTMLSelectElement>): void {
    event.preventDefault();
    setSelectedProjectId(Number(event.target.value));
  }

  return (
    <>
      <label>
        See project's time registrations:
        <select value={selectedProjectId} onChange={handleSelectProject}>
          <option key={"none"} value={0}>
            {"none"}
          </option>
          {projects.map((project) => {
            return (
              <option key={project.id} value={project.id}>
                {project.name}
              </option>
            );
          })}
        </select>
      </label>
      {selectedProjectId > 0 && (
        <table className="table-fixed w-full">
          <caption>Time registrations for selected project</caption>
          <thead className="bg-gray-200">
            <tr>
              <th sortable-column className="border px-4 py-2 w-12">
                #
              </th>
              <th className="border px-4 py-2">Time spent </th>
              <th className="border px-4 py-2">Registration Added</th>
            </tr>
          </thead>
          <tbody>
            {timeRegistrations
              .filter((timeRegistration) => timeRegistration.projectId == selectedProjectId)
              .map((timeRegistration) => {
                return (
                  <tr key={timeRegistration.id}>
                    <td className="border px-4 py-2 w-12"> {timeRegistration.id}</td>
                    <td className="border px-4 py-2">{timeInMinutesToFormattedString(timeRegistration.timeSpentInMinutes)}</td>
                    <td className="border px-4 py-2">{timeRegistration.registrationCreated.toLocaleDateString()}</td>
                  </tr>
                );
              })}
          </tbody>
        </table>
      )}
    </>
  );
}
