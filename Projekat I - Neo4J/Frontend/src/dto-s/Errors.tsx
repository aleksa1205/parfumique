export class WrongCredentials extends Error {
  constructor(message?: string) {
    super(message || "Unauthorized");
    this.name = "Unauthorized";
  }
}

export class UsernameExists extends Error {
  constructor(message?: string) {
    super(message || "Username already in use!");
    this.name = "UsernameExists";
  }
}

export class NotFoundError extends Error {
  constructor(message?: string) {
    super(message || "Not Found");
    this.name = "NotFound";
  }
}
