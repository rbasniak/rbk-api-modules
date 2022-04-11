export interface LogsEntry {
  id: string;
  timestamp: string;
  message: string;
  level: LogLevel;
  applicationLayer: string;
  applicationArea: string;
  applicationVersion: string;
  source: string;
  inputData: string;
  enviroment: string;
  enviromentVersion: string;
  username: string;
  domain: string;
  machineName: string;
}

export enum LogLevel {
  Warning = 0,
  Error,
  Debug
}