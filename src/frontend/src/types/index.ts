export type Role = 'Admin' | 'Roommate';
export interface AuthState { token: string; email: string; role: Role; fullName: string; }
export interface InventoryItem { id: string; name: string; description: string; category: string; quantity: number; unit: string; minimumStockThreshold: number; expiryDateUtc?: string; location: string; isLowStock: boolean; isOutOfStock: boolean; }
