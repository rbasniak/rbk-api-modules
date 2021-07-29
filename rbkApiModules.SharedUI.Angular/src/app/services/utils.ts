export function getUrl(): string {
  const url = window.location.href.split('/shared-ui')[0];
  return url;
}