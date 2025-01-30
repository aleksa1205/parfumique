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

export function fileToBase64(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
      const reader = new FileReader();

      reader.onload = () => {
          if (typeof reader.result === 'string') {
              const base64String = reader.result.split(',')[1]; // Remove the data URL prefix
              const sanitizedBase64String = base64String.replace(/\s/g, ''); // Remove whitespace characters
              resolve(sanitizedBase64String);
          } else {
              reject(new Error('Failed to convert file to base64'));
          }
      };

      reader.onerror = (error) => {
          reject(error);
      };

      reader.readAsDataURL(file);
  });
};

export function formatBase64(file : string) {
  return file.split(',')[1].replace(/\s/g, '');
}

export function stringToEnum<T extends Record<string, number | string>>(enumObj: T, value: string): T[keyof T] | undefined {
  return (Object.keys(enumObj) as Array<keyof T>).includes(value as keyof T) ? (enumObj[value as keyof T]) : undefined;
}

export function crop(url : string, aspectRatio: number) : Promise<HTMLCanvasElement> {
  // we return a Promise that gets resolved with our canvas element
  return new Promise((resolve) => {
      // this image will hold our source image data
      const inputImage = new Image();

      // we want to wait for our image to load
      inputImage.onload = () => {
          // let's store the width and height of our image
          const inputWidth = inputImage.naturalWidth;
          const inputHeight = inputImage.naturalHeight;

          // get the aspect ratio of the input image
          const inputImageAspectRatio = inputWidth / inputHeight;

          // if it's bigger than our target aspect ratio
          let outputWidth = inputWidth;
          let outputHeight = inputHeight;
          if (inputImageAspectRatio > aspectRatio) {
              outputWidth = inputHeight * aspectRatio;
          } else if (inputImageAspectRatio < aspectRatio) {
              outputHeight = inputWidth / aspectRatio;
          }

          // calculate the position to draw the image at
          const outputX = (outputWidth - inputWidth) * 0.5;
          const outputY = (outputHeight - inputHeight) * 0.5;

          // create a canvas that will present the output image
          const outputImage = document.createElement('canvas');

          // set it to the same size as the image
          outputImage.width = outputWidth;
          outputImage.height = outputHeight;

          // draw our image at position 0, 0 on the canvas
          const ctx = outputImage.getContext('2d');
          if(ctx !== null)
              ctx.drawImage(inputImage, outputX, outputY);
          resolve(outputImage);
      };

      // start loading our image
      inputImage.src = url;
  });
}