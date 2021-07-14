import { AnalyticsEntry } from './analytics-entry';
import { ChartDefinition } from '../chart-definition';

export interface SearchResults {
  searchResults: AnalyticsEntry[];
  charts: ChartDefinition[];
}