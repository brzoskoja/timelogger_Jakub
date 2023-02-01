import React, { useState } from "react";
import { postAddProject } from "../api/ApiCalls";
import { Project } from "../dataEntities/Project";
interface AddProjectProps {
  projects: Array<Project>;
  setError(error: string): void;
  fetchData(): void;
}

export default function AddProject({ projects, setError, fetchData }: AddProjectProps) {
  const [projectName, setProjectName] = useState("");
  const [customerName, setCustomerName] = useState("");
  const [deadline, setDeadline] = useState(getTodaysDate());

  function handleProjectNameUpdate(event: React.ChangeEvent<HTMLInputElement>): void {
    event.preventDefault;
    setProjectName(event.target.value);
    setError("");
  }

  function handleCustomerNameUpdate(event: React.ChangeEvent<HTMLInputElement>): void {
    event.preventDefault;
    setCustomerName(event.target.value);
  }

  function handleDeadlineUpdate(event: React.ChangeEvent<HTMLInputElement>): void {
    event.preventDefault;
    setDeadline(event.target.value);
  }

  function getTodaysDate(): string {
    return new Date().toISOString().split("T")[0];
  }

  async function handleAddProjectButtonClick(event: React.MouseEvent<HTMLButtonElement>): Promise<void> {
    event.preventDefault();
    if (projects.filter((project) => project.name === projectName).length > 0) {
      setError("This project already exists");
    } else if (projectName.length === 0) {
      setError("Project needs a name");
    } else {
      const success = await postAddProject(projectName, customerName, deadline);
      if (success) {
        fetchData();
        setCustomerName("");
        setProjectName("");
        setError("");
        setDeadline(getTodaysDate());
      } else {
        setError("Something went wrong when creating the new project");
      }
    }
  }

  return (
    <div style={{ color: "green" }} className="flex justify-content: space-around w-1/2">
      <button onClick={handleAddProjectButtonClick} className="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded">
        Register new project
      </button>
      <div>
        <label>
          Project Name :
          <input className="square border" type="text" value={projectName} onChange={handleProjectNameUpdate} />
        </label>
      </div>
      <div>
        <label>
          Customer Name:
          <input className="square border" type="text" value={customerName} onChange={handleCustomerNameUpdate} />
        </label>
      </div>
      <div>
        <label>
          Deadline:
          <input type="date" value={deadline} min={getTodaysDate()} onChange={handleDeadlineUpdate}></input>
        </label>
      </div>
    </div>
  );
}
