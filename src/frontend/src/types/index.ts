export type Role = 'Admin' | 'Roommate';
export interface AuthState { token: string; email: string; role: Role; fullName: string; }
export interface InventoryItem { id: string; name: string; description: string; category: string; quantity: number; unit: string; minimumStockThreshold: number; expiryDateUtc?: string; location: string; isLowStock: boolean; isOutOfStock: boolean; }
export interface ShoppingListItem { id: string; name: string; quantity: number; unit: string; notes?: string; isCompleted: boolean; inventoryItemId?: string; }
export interface DashboardDto { totalItems: number; lowStockCount: number; outOfStockCount: number; expiringSoonCount: number; shoppingListCount: number; recentActivity: unknown[]; }
export interface AuditEntry { id: string; action: string; category: string; summary: string; userEmail: string; timestampUtc: string; }
export interface UserDto { id: string; email: string; fullName: string; roles: string[]; isActive: boolean; }
