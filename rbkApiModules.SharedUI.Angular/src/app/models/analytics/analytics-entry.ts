export interface AnalyticsEntry {
  action: string;
  area: string;
  domain: string;
  duration: number;
  id: string;
  identity: string;
  ipAddress: string;
  method: string;
  path: string;
  requestSize: number;
  response: number;
  responseSize: number;
  timestamp: string;
  totalTransactionTime: number;
  transactionCount: number;
  userAgent: string;
  username: string;
  version: string;
  wasCached: boolean;
}