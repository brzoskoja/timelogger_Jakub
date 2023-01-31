export const timeInMinutesToFormattedString = (minutes: number): string => {
  return (minutes / 60 > 0 ? Math.floor(minutes / 60) + "h " : "") + (minutes % 60) + "min";
};
