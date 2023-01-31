import React, { useState } from "react";
import { postAddTime } from "../api/ApiCalls";
import { Project } from "../dataEntities/Project";
interface AddTimeToProjectsProps {
  projects: Array<Project>;
  setError(error: string): void;
  fetchData(): void;
}

export default function AddTimeToProjects({ projects, setError, fetchData }: AddTimeToProjectsProps) {
  const [registerTimeButtonText, setRegisterTimeButtonText] = useState("Add new time entry");
  const [addTimeHidden, setAddTimeHidden] = useState(true);
  const [selectedProject, setSelectedProject] = useState("");
  const [minutesToAdd, setMinutesToAdd] = useState("0");

  async function handleAddTimeButtonClick(event: React.MouseEvent<HTMLButtonElement>): Promise<void> {
    event.preventDefault();
    if (addTimeHidden) {
      setAddTimeHidden(false);
      setRegisterTimeButtonText("Confirm the input");
    } else {
      if (validateAddTime()) {
        const success = await postAddTime(minutesToAdd, selectedProject);
        if (success) {
          fetchData();
          refreshAll();
        } else {
          setError("Something went wrong when posting the new time entry");
        }
      }
    }
  }

  function refreshAll() {
    setError("");
    setMinutesToAdd("0");
    setSelectedProject("");
    setRegisterTimeButtonText("Add new time entry");
    setAddTimeHidden(true);
  }

  function validateAddTime(): boolean {
    const minutes = Number(minutesToAdd);
    if (Number.isNaN(minutes)) {
      setError("Input provided is not a number");
      return false;
    }
    if (minutes < 30) {
      setError("Minimal time registration is 30 min");
      return false;
    }
    if (!selectedProject) {
      setError("Please select a different project");
      return false;
    }
    return true;
  }

  function handleSelectProject(event: React.ChangeEvent<HTMLSelectElement>): void {
    event.preventDefault();
    setSelectedProject(event.target.value);
  }

  function handleMinutesToAdd(event: React.ChangeEvent<HTMLInputElement>): void {
    event.preventDefault();
    setMinutesToAdd(event.target.value);
  }

  return (
    <div className="flex justify-content: space-around w-1/2">
      <button onClick={handleAddTimeButtonClick} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
        {registerTimeButtonText}
      </button>
      {!addTimeHidden && (
        <>
          <div>
            <label>
              Project:
              <select value={selectedProject} onChange={handleSelectProject}>
                <option disabled={selectedProject != ""} key={"Select project"} value={"Select project"}>
                  {"Select project"}
                </option>
                {projects
                  .filter((project) => !project.isCompleted)
                  .map((project) => {
                    return (
                      <option key={project.name} value={project.name}>
                        {project.name}
                      </option>
                    );
                  })}
              </select>
            </label>
          </div>
          <div>
            <label>
              Minutes spent:
              <input type="number" value={minutesToAdd} onChange={handleMinutesToAdd} />
            </label>
          </div>
        </>
      )}
    </div>
  );
}
