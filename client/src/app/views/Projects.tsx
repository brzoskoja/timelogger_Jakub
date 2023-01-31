import React, { useState, useEffect } from "react";
import { getAllProjects, getAllTimeRegistrations } from "../api/ApiCalls";
import { Project } from "../dataEntities/Project";
import ProjectTable from "../components/ProjectTable";
import AddTimeToProjects from "../components/AddTimeToProject";
import TimeRegistrationTable from "../components/TimeRegistrationTable";
import { TimeRegistration } from "../dataEntities/TimeRegistration";

export default function Projects() {
  const [projects, setProjects] = useState(Array<Project>);
  const [timeRegistrations, setTimeRegistrations] = useState(Array<TimeRegistration>);
  const [error, setError] = useState("");

  const fetchData = async () => {
    try {
      const allProjects = await getAllProjects();
      setProjects(allProjects);
      const allRegistrations = await getAllTimeRegistrations();
      setTimeRegistrations(allRegistrations);
    } catch (e) {
      setError(e as string);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <>
      {error.length > 0 && (
        <div className="flex justify-center">
          <div style={{ fontSize: "25", color: "red" }}>{error}</div>
        </div>
      )}
      <div className="flex items-center my-6">
        <AddTimeToProjects projects={projects} setError={setError} fetchData={fetchData} />

        <div className="w-1/2 flex justify-end">
          <form>
            <input className="border rounded-full py-2 px-4" type="search" placeholder="Search" aria-label="Search" />
            <button className="bg-blue-500 hover:bg-blue-700 text-white rounded-full py-2 px-4 ml-2" type="submit">
              Search
            </button>
          </form>
        </div>
      </div>

      <ProjectTable projects={projects} />
      <TimeRegistrationTable timeRegistrations={timeRegistrations} projects={projects} />
    </>
  );
}
