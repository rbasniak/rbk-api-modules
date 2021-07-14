import { ChartDefinition } from '@models/chart-definition';
import { DiagnosticsEntry } from './diagnostics-entry';

export interface SearchResults {
  searchResults: DiagnosticsEntry[];
  charts: ChartDefinition[];
}