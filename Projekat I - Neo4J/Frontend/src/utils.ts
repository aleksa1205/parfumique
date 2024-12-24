export function base64ToUrl(data: string): string {
  const binaryData = atob(data);

  const arrayBuffer = new ArrayBuffer(binaryData.length);
  const uint8Array = new Uint8Array(arrayBuffer);
  for (let i = 0; i < binaryData.length; i++) {
    uint8Array[i] = binaryData.charCodeAt(i);
  }

  const blob = new Blob([uint8Array], { type: "image/png" });
  const url = URL.createObjectURL(blob);

  return url;
}
