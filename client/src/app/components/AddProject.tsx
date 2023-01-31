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

  function handleProjectNameUpdate(event: React.ChangeEvent<HTMLInputElement>): void {
    event.preventDefault;
    setProjectName(event.target.value);
    setError("");
  }

  function handleCustomerNameUpdate(event: React.ChangeEvent<HTMLInputElement>): void {
    event.preventDefault;
    setCustomerName(event.target.value);
  }

  async function handleAddProjectButtonClick(event: React.MouseEvent<HTMLButtonElement>): Promise<void> {
    event.preventDefault();
    if (projects.filter((project) => project.name === projectName).length > 0) {
      setError("This project already exists");
    } else {
      const success = await postAddProject(projectName, customerName);
      if (success) {
        fetchData();
        setCustomerName("");
        setProjectName("");
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
          <input type="date" value="2018-07-22" min="2018-01-01" max="2018-12-31"></input>
        </label>
      </div>
    </div>
  );
}
