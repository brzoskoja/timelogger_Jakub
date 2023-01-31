import { Project } from "../dataEntities/Project";
import { TimeRegistration } from "../dataEntities/TimeRegistration";

const BASE_URL = "http://localhost:3001/api";

export const getAllProjects = async () : Promise<Project[]> => {
    const response = await fetch(`${BASE_URL}/projects`);
        if(!response.ok){
      throw response.statusText;
    }
    const projectsDto =  await response.json() as Project[];
    // this is done to assure proper date mapping
    return  projectsDto.map(project =>{
        project.deadline = new Date(project.deadline);
        return project;
    })
}

export const getAllTimeRegistrations = async () : Promise<TimeRegistration[]> => {
    const response = await fetch(`${BASE_URL}/projects/getTimeRegistrations`);
    if(!response.ok){
      throw response.statusText;
    }
    const timeRegistrationsDto =  await response.json() as TimeRegistration[];
    // this is done to assure proper date mapping
    return  timeRegistrationsDto.map(timeRegistration =>{
        timeRegistration.registrationCreated = new Date(timeRegistration.registrationCreated);
        return timeRegistration;
    })
}

export const postAddTime = async(minutes: string, project: string): Promise<boolean> =>{
  try{
    
const response = await fetch(`${BASE_URL}/projects/addTimeEntry`, {
  method: 'POST',
  headers:{
      'Content-Type': 'application/json'
    },
  body: JSON.stringify({"minutes": minutes, "project" : project}),
});

 if (!response.ok) {
  return false;
}
  return true;
}catch(e){
  return false;
}
}


