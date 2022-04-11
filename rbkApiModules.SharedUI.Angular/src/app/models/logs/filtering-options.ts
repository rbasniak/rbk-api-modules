import { LogLevel } from './logs-entry';

export interface FilteringOptions {
  messages: string[];
  levels: LogLevel[];
  layers: string[];
  areas: string[];
  versions: string[];
  sources: string[];
  enviroments: string[];
  enviromentsVersions: string[];
  users: string[];
  domains: string[];
  machines: string[];
  endDate: Date;
  startDate: Date;
}