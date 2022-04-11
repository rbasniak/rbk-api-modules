import { ChartDefinition } from '@models/chart-definition';
import { LogsEntry } from './logs-entry';

export interface SearchResults {
  searchResults: LogsEntry[];
  charts: ChartDefinition[];
}